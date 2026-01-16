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
using Microsoft.Extensions.Options;
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
using Swashbuckle.AspNetCore.SwaggerGen;

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

        services.AddSwaggerGen(options =>
        {
            // keep conflict resolver; actual SwaggerDoc registration moved into ConfigureSwaggerOptions
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            // keep operation filter registration here (optional - ConfigureSwaggerOptions also registers it)
            options.OperationFilter<SwaggerHeaderFilter>();
        });
        // Register the IConfigureOptions<SwaggerGenOptions> so Swagger documents are created using the ApiVersionDescriptionProvider
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    }

    // Non-static so the ApiVersionDescriptionProvider can be injected
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            var ordered = provider.ApiVersionDescriptions
                .OrderBy(d => d.GroupName, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(d => d.ApiVersion);

            foreach (var description in ordered)
            {
                var docName = $"{description.GroupName}V{description.ApiVersion.MajorVersion}";
                options.SwaggerEndpoint($"/swagger/{docName}/swagger.json", $"{description.GroupName} V{description.ApiVersion.MajorVersion}");
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