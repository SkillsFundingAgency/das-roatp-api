using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.CourseManagement.Jobs;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi;
using SFA.DAS.Roatp.Jobs.Infrastructure.Tokens;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    [ExcludeFromCodeCoverage]
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            BuildConfiguration(builder);
            BuildHttpClients(builder);
            AddNLog(builder);
        }

        private static void AddNLog(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging((options) =>
            {
                options.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole();

                NLogConfiguration.ConfigureNLog();
            });
        }

        private static void BuildConfiguration(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
         
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(configuration);
            
            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = new[] { "SFA.DAS.Roatp.Jobs" };
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });

            var config = configBuilder.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            builder.Services.AddOptions(); 

            builder.Services.Configure<RoatpV2ApiConfiguration>(config.GetSection(nameof(RoatpV2ApiConfiguration)));
            builder.Services.Configure<CoursesApiConfiguration>(config.GetSection(nameof(CoursesApiConfiguration)));
        }

         private static void BuildHttpClients(IFunctionsHostBuilder builder)
         {
             var acceptHeaderName = "Accept";
             var acceptHeaderValue = "application/json";
             var handlerLifeTime = TimeSpan.FromMinutes(5);
        
             builder.Services.AddHttpClient<IGetActiveStandardsApiClient, GetActiveStandardsApiClient>((serviceProvider, httpClient) =>
                 {
                     var coursesApiConfiguration = serviceProvider.GetService<IOptions<CoursesApiConfiguration>>().Value;
                     httpClient.BaseAddress = new Uri(coursesApiConfiguration.Url);

                     httpClient.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
                     httpClient.DefaultRequestHeaders.Add("X-Version","1"); 
             
                     var configuration = serviceProvider.GetService<IConfiguration>();
                     if (!configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                     {
                         var generateTokenTask = BearerTokenGenerator.GenerateTokenAsync(coursesApiConfiguration.Identifier);
                         httpClient.DefaultRequestHeaders.Authorization = generateTokenTask.GetAwaiter().GetResult();
                     }
                 })
                 .SetHandlerLifetime(handlerLifeTime);
             
             builder.Services.AddHttpClient<IReloadStandardsApiClient, ReloadStandardsApiClient>((serviceProvider, httpClient) =>
                 {
                      var roatpV2ApiConfiguration = serviceProvider.GetService<IOptions<RoatpV2ApiConfiguration>>().Value;
                      httpClient.BaseAddress = new Uri(roatpV2ApiConfiguration.Url);
                      
                     httpClient.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
                     httpClient.DefaultRequestHeaders.Add("X-Version", "1"); 
        
                     var configuration = serviceProvider.GetService<IConfiguration>();
                     if (!configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                     {
                         var generateTokenTask = BearerTokenGenerator.GenerateTokenAsync(roatpV2ApiConfiguration.Identifier);
                         httpClient.DefaultRequestHeaders.Authorization = generateTokenTask.GetAwaiter().GetResult();
                     }
                 })
                 .SetHandlerLifetime(handlerLifeTime);
        }
    }
}
