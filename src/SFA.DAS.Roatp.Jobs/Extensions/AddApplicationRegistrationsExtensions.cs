using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Extensions;
public static class AddApplicationRegistrationsExtension
{
    public static IServiceCollection AddApplicationRegistrations(this IServiceCollection services)
    {
        services.AddTransient<IReloadStandardsCacheService, ReloadStandardsCacheService>();
        services.AddTransient<IReloadProviderRegistrationDetailService, ReloadProviderRegistrationDetailService>();
        services.AddTransient<ILoadUkrlpAddressesService, LoadUkrlpAddressesService>();
        services.AddTransient<IUpdateProviderAddressCoordinatesService, UpdateProviderAddressCoordinatesService>();
        services.AddTransient<IDataExtractorService, DataExtractorService>();
        services.AddTransient<IImportNationalAchievementRateOverallService, ImportNationalAchievementRateOverallService>();
        services.AddTransient<IImportNationalAchievementRateService, ImportNationalAchievementRateService>();

        return services;
    }
}