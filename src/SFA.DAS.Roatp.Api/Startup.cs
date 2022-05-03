using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Roatp.Api.HealthCheck;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Data.Extensions;
using SFA.DAS.Roatp.Application.Extensions;

namespace SFA.DAS.Roatp.Api
{
    public class Startup
    {
        private readonly string _initialEnvironment;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            _initialEnvironment = configuration["Environment"];

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
            if (!IsEnvironmentLocalOrDev)
            {
                var azureAdConfiguration = Configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, "Default"}
                };

                services.AddAuthentication(azureAdConfiguration, policies);
            }

            services
                .AddHealthChecks()
                .AddDbContextCheck<RoatpDataContext>()
                .AddCheck<StandardsHealthCheck>(StandardsHealthCheck.HealthCheckResultDescription,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready" });

            services.AddRoatpDataContext(Configuration["SqlDatabaseConnectionString"], _initialEnvironment);

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddApplicationRegistrations();

            services
                .AddControllers(o =>
                {
                    if (!IsEnvironmentLocalOrDev) o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "RoatpAPI", Version = "v1" });
                options.OperationFilter<SwaggerVersionHeaderFilter>();
            });

            services.AddTransient<IGetProviderCoursesService, GetProviderCoursesService>();
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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

          

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
            });

            if (!IsEnvironmentLocalOrDev)
            {

                app.UseHealthChecks("/ping", new HealthCheckOptions
                {
                    Predicate = (_) => false,
                    ResponseWriter = (context, report) =>
                    {
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("");
                    }
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool IsEnvironmentLocalOrDev
            => Configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
            || Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}
