using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Extensions;

public static class AddApplicationRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IReloadStandardsCacheService, ReloadStandardsCacheService>();
        services.AddTransient<IReloadProviderRegistrationDetailService, ReloadProviderRegistrationDetailService>();
        services.AddTransient<ILoadUkrlpAddressesService, LoadUkrlpAddressesService>();
        services.AddTransient<IUpdateProviderAddressCoordinatesService, UpdateProviderAddressCoordinatesService>();
        services.AddTransient<IDataExtractorService, DataExtractorService>();
        services.AddTransient<IImportNationalAchievementRateOverallService, ImportNationalAchievementRateOverallService>();
        services.AddTransient<IImportNationalAchievementRateService, ImportNationalAchievementRateService>();

        services.AddTransient<IImportAnnualFeedbackSummariesService, ImportAnnualFeedbackSummariesService>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        RegisterHttpClient(services, configuration);

        return services;
    }

    private static void RegisterHttpClient(IServiceCollection services, IConfiguration configuration)
    {
        var handlerLifeTime = TimeSpan.FromMinutes(5);
        services.AddTransient<ICourseManagementOuterApiClient, CourseManagementOuterApiClient>();
        services.AddHttpClient<ICourseManagementOuterApiClient, CourseManagementOuterApiClient>(httpClient =>
        {
            var apiConfig = configuration
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
