using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.Data.Repositories;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class AddRoatpDataContextExtension
{
    public static IServiceCollection AddRoatpDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoatpDataContext>((serviceProvider, options) =>
        {
            var sqlConnectionString = configuration["SqlConnectionString"]!;
            var connection = new SqlConnection(sqlConnectionString);

            options.UseSqlServer(
                connection,
                o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(20), null));
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
        services.AddTransient<INationalAchievementRatesReadRepository, NationalAchievementRatesReadRepository>();
        services.AddTransient<IProvidersCountReadRepository, ProvidersCountReadRepository>();
        services.AddTransient<IShortlistsRepository, ShortlistsRepository>();
        services.AddTransient<INationalQarReadRepository, NationalQarReadRepository>();
        services.AddTransient<IProviderEmployerStarsReadRepository, ProviderEmployerStarsReadRepository>();
        services.AddTransient<ICourseProviderDetailsReadRepository, CourseProviderDetailsReadRepository>();
        services.AddTransient<IImportFeedbackSummariesRepository, ImportFeedbackSummariesRepository>();
        services.AddTransient<IProviderContactsReadRepository, ProviderContactReadRepository>();
        services.AddTransient<IProviderContactsWriteRepository, ProviderContactsWriteRepository>();
        services.AddTransient<IReloadProviderCourseTypesRepository, ReloadProviderCourseTypesRepository>();
        services.AddTransient<IProviderCourseTypesReadRepository, ProviderCourseTypesReadRepository>();
        services.AddTransient<IProviderCoursesTimelineRepository, ProviderCoursesTimelineRepository>();
    }
}
