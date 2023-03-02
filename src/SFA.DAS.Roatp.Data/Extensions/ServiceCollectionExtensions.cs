﻿using System;
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
            services.AddTransient<IProviderCoursesReadRepository, ProviderCoursesReadRepository>();
            services.AddTransient<IProviderCoursesWriteRepository, ProviderCoursesWriteRepository>();
            services.AddTransient<IRegionsReadRepository, RegionsReadRepository>();
            services.AddTransient<IProvidersWriteRepository, ProvidersWriteRepository>();
            services.AddTransient<IReloadStandardsRepository, ReloadStandardsRepository>();
            services.AddTransient<IProviderLocationsReadRepository, ProviderLocationsReadRepository>();
            services.AddTransient<IProviderLocationsBulkRepository, ProviderLocationsBulkRepository>();
            services.AddTransient<IReloadProviderRegistrationDetailsRepository, ReloadProviderRegistrationDetailsRepository>();
            services.AddTransient<IProviderCourseLocationsReadRepository, ProviderCourseLocationsReadRepository>();
            services.AddTransient<IProviderCourseLocationsWriteRepository, ProviderCourseLocationsWriteRepository>();
            services.AddTransient<IProviderCourseLocationsBulkRepository, ProviderCourseLocationsBulkRepository>();
            services.AddTransient<ILoadProviderRepository, LoadProviderRepository>();
            services.AddTransient<IProviderRegistrationDetailsReadRepository, ProviderRegistrationDetailsReadRepository>();
            services.AddTransient<IProviderRegistrationDetailsWriteRepository, ProviderRegistrationDetailsWriteRepository>();
            services.AddTransient<IProviderLocationsWriteRepository, ProviderLocationsWriteRepository>();
            services.AddTransient<IProviderCourseLocationsWriteRepository, ProviderCourseLocationsWriteRepository>();
            services.AddTransient<IImportAuditWriteRepository, ImportAuditWriteRepository>();
            services.AddTransient<IImportAuditReadRepository, ImportAuditReadRepository>();
            services.AddTransient<INationalAchievementRatesImportWriteRepository, NationalAchievementRatesImportWriteRepository>();
            services.AddTransient<INationalAchievementRatesImportReadRepository, NationalAchievementRatesImportReadRepository>();
            services.AddTransient<INationalAchievementRatesWriteRepository, NationalAchievementRatesWriteRepository>();
            services.AddTransient<INationalAchievementRatesOverallImportWriteRepository, NationalAchievementRatesOverallImportWriteRepository>();
            services.AddTransient<INationalAchievementRatesOverallImportReadRepository, NationalAchievementRatesOverallImportReadRepository>();
            services.AddTransient<INationalAchievementRatesOverallWriteRepository, NationalAchievementRatesOverallWriteRepository>();
            services.AddTransient<INationalAchievementRatesOverallReadRepository, NationalAchievementRatesOverallReadRepository>();
            services.AddTransient<IReloadProviderAddressesRepository, ReloadProviderAddressesRepository>();
            services.AddTransient<IProviderAddressReadRepository, ProviderAddressReadRepository>();
            services.AddTransient<IProviderAddressWriteRepository, ProviderAddressWriteRepository>();
            services.AddTransient<IProviderDetailsReadRepository, ProviderDetailsReadRepository>();
            services.AddTransient<INationalAchievementRatesReadRepository, NationalAchievementRatesReadRepository>();
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
