using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
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
            .AddLogging()
            .AddApplicationInsightsTelemetry()
            .AddTelemetryNotFoundAsSuccessfulResponse();

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
            opt.DefaultApiVersion = new ApiVersion(1, 0);
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
            options.SwaggerDoc(Constants.EndpointGroups.Management, new OpenApiInfo { Title = "Course Management", Version = "v1" });
            options.SwaggerDoc(Constants.EndpointGroups.Integration, new OpenApiInfo { Title = "Roatp Integration", Version = "v1" });
            options.OperationFilter<SwaggerHeaderFilter>();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{Constants.EndpointGroups.Management}/swagger.json", Constants.EndpointGroups.Management);
            options.SwaggerEndpoint($"/swagger/{Constants.EndpointGroups.Integration}/swagger.json", Constants.EndpointGroups.Integration);
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
