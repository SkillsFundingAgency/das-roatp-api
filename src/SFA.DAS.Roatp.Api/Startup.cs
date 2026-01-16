using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.Api.AppStart;
using SFA.DAS.Roatp.Api.HealthCheck;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Extensions;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Data.Extensions;
using SFA.DAS.Telemetry.Startup;

namespace SFA.DAS.Roatp.Api;

[ExcludeFromCodeCoverage]
public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration);

        config.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["Environment"];
            options.PreFixConfigurationKeys = false;
        });

        Configuration = config.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddApplicationInsightsTelemetry()
            .AddTelemetryNotFoundAsSuccessfulResponse();

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        if (!IsEnvironmentLocalOrDev)
        {
            var azureAdConfiguration = Configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string>
            {
                {Constants.EndpointGroups.Management, Constants.EndpointGroups.Management},
                {Constants.EndpointGroups.Integration, Constants.EndpointGroups.Integration }
            };

            services.AddAuthentication(azureAdConfiguration, policies);
        }

        services
            .AddHealthChecks()
            .AddDbContextCheck<RoatpDataContext>()
            .AddCheck<StandardsHealthCheck>(StandardsHealthCheck.HealthCheckResultDescription,
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready" });

        services.AddRoatpDataContext(Configuration);

        services.AddApiVersioning(opt =>
        {

            opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            opt.AssumeDefaultVersionWhenUnspecified = false; // prevent blanket default
            opt.ReportApiVersions = true;

        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddApplicationRegistrations();

        services
            .AddControllers(o =>
            {
                if (!IsEnvironmentLocalOrDev)
                    o.Conventions.Add(new AuthorizeByPathControllerModelConvention());
                o.Conventions.Add(new ApiExplorerGroupingConvention());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        // Replace the existing services.AddSwaggerGen(...) block with this:
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            // Temporary provider to get ApiVersionDescriptions at startup.
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            // Register a document for every ApiVersionDescription (group + version).
            foreach (var apiVersionDescription in provider.ApiVersionDescriptions)
            {
                var docName = $"{apiVersionDescription.GroupName}-v{apiVersionDescription.ApiVersion}";
                options.SwaggerDoc(docName, new OpenApiInfo
                {
                    Title = $"Course {apiVersionDescription.GroupName} v{apiVersionDescription.ApiVersion}",
                    Version = apiVersionDescription.ApiVersion.ToString()
                });
            }

            options.OperationFilter<SwaggerHeaderFilter>();
        });
    }

    // Non-static so the ApiVersionDescriptionProvider can be injected
    // Non-static so the ApiVersionDescriptionProvider can be injected
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();

        // Log what ApiExplorer reports
        logger.LogInformation("ApiVersionDescriptions count: {Count}", provider.ApiVersionDescriptions.Count());
        foreach (var d in provider.ApiVersionDescriptions)
        {
            logger.LogInformation("ApiVersionDescription: GroupName={GroupName}, ApiVersion={ApiVersion}, IsDeprecated={IsDeprecated}",
                d.GroupName, d.ApiVersion, d.IsDeprecated);
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            // Register an endpoint for every ApiVersionDescription using the same docName format.
            var ordered = provider.ApiVersionDescriptions
                .OrderBy(d => d.GroupName, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(d => d.ApiVersion);

            foreach (var description in ordered)
            {
                var docName = $"{description.GroupName}-v{description.ApiVersion}";
                var url = $"/swagger/{docName}/swagger.json";
                logger.LogInformation("Registering Swagger endpoint: {Url} -> {Title}", url, $"Course {description.GroupName} v{description.ApiVersion}");
                options.SwaggerEndpoint(url, $"Course {description.GroupName} v{description.ApiVersion}");
            }

            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseHealthChecks();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private bool IsEnvironmentLocalOrDev
        => Configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
        || Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
}