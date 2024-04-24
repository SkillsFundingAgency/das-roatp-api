using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Roatp.Data.Extensions;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(
        builder =>
        {
            builder.AddConfiguration();
        })
    .ConfigureServices(
        (context, services) =>
        {
            services
                .AddApplicationInsightsTelemetryWorkerService()
                .ConfigureFunctionsApplicationInsights()
                .AddOptions()
                .AddRoatpDataContext(context.Configuration["SqlDatabaseConnectionString"],
                    context.Configuration["EnvironmentName"])
                .AddApplicationRegistrations()
                .RegisterHttpClient(context.Configuration)
                .AddLogging((options) =>
                {
                    options.AddFilter(typeof(Program).Namespace, LogLevel.Information);
                    options.SetMinimumLevel(LogLevel.Trace);
                    options.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });


                    NLogConfiguration.ConfigureNLog();
                });
        })
    .Build();

await host.RunAsync();