using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRoatpDataContext(this IServiceCollection services, string connectionString, string environmentName)
        {
            services.AddDbContext<RoatpDataContext>((serviceProvider, options) =>
            {
                var connection = new SqlConnection(connectionString);

                if (!environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                {
                    var generateTokenTask = GenerateTokenAsync();
                    connection.AccessToken = generateTokenTask.GetAwaiter().GetResult();
                }
                else
                {
                    options.EnableSensitiveDataLogging();
                }

                options.UseSqlServer(
                    connection,
                    o => o.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds));
            });
            RegisterServices(services);
            return services;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IStandardsReadRepository, StandardsReadRepository>();
            services.AddTransient<IProvidersReadRepository, ProvidersReadRepository>();
            services.AddTransient<IProviderCourseReadRepository, ProviderCourseReadRepository>();
            services.AddTransient<IProviderCoursesWriteRepository, ProviderCoursesWriteRepository>();
            services.AddTransient<IRegionsReadRepository, RegionsReadRepository>();
            services.AddTransient<IProvidersWriteRepository, ProvidersWriteRepository>();
            services.AddTransient<IReloadStandardsRepository, ReloadStandardsRepository>();
            services.AddTransient<IGetStandardsCountRepository, GetStandardsCountRepository>();
            services.AddTransient<IProviderLocationsReadRepository, ProviderLocationsReadRepository>();
            services.AddTransient<IProviderLocationsBulkRepository, ProviderLocationsBulkRepository>();
            services.AddTransient<IReloadProviderRegistrationDetailsRepository, ReloadProviderRegistrationDetailsRepository>();
            services.AddTransient<IProviderCourseLocationReadRepository, ProviderCourseLocationReadRepository>();
            services.AddTransient<IProviderCourseLocationsDeleteRepository, ProviderCourseLocationsDeleteRepository>();
            services.AddTransient<IProviderCourseLocationsInsertRepository, ProviderCourseLocationsInsertRepository>();
            services.AddTransient<ILoadProviderRepository, LoadProviderRepository>();
            services.AddTransient<IGetActiveProviderRegistrationsRepository, GetActiveProviderRegistrationsRepository>();
            services.AddTransient<IProviderLocationsWriteRepository, ProviderLocationsWriteRepository>();
            services.AddTransient<IProviderCourseLocationWriteRepository, ProviderCourseLocationWriteRepository>();
        }

        public static async Task<string> GenerateTokenAsync()
        {
            const string AzureResource = "https://database.windows.net/";
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);

            return accessToken;
        }
    }
}
