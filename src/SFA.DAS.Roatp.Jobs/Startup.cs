using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.CourseManagement.Jobs;
using SFA.DAS.Roatp.Data.Extensions;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Services;
using System;
using System.Diagnostics.CodeAnalysis;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SFA.DAS.Roatp.CourseManagement.Jobs
{
    [ExcludeFromCodeCoverage]
    internal class Startup : FunctionsStartup
    {
        IConfigurationRoot _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            BuildConfiguration(builder);
            ConfigureHttpClient(builder.Services);
            AddNLog(builder);
        }

        private void AddNLog(IFunctionsHostBuilder builder)
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

        private void BuildConfiguration(IFunctionsHostBuilder builder)
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

            _configuration = configBuilder.Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), _configuration));
            
            builder.Services.AddRoatpDataContext(_configuration["SqlDatabaseConnectionString"], _configuration["EnvironmentName"]);

            builder.Services.AddTransient<IReloadStandardsCacheService, ReloadStandardsCacheService>();
            builder.Services.AddTransient<IReloadProviderRegistrationDetailService, ReloadProviderRegistrationDetailService>();
            builder.Services.AddTransient<ILoadCourseDirectoryDataService, LoadCourseDirectoryDataService>();
        }

        private void ConfigureHttpClient(IServiceCollection services)
        {
            var handlerLifeTime = TimeSpan.FromMinutes(5);
            services.AddHttpClient<ICourseManagementOuterApiClient, CourseManagementOuterApiClient>(httpClient =>
            {
                var apiConfig = _configuration
                    .GetSection("RoatpCourseManagementOuterApi")
                    .Get<RoatpCourseManagementOuterApiConfiguration>();

                httpClient.BaseAddress = new Uri(apiConfig.BaseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-Version", "1");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiConfig.SubscriptionKey);
            })
           .SetHandlerLifetime(handlerLifeTime);
        }
    }
}
