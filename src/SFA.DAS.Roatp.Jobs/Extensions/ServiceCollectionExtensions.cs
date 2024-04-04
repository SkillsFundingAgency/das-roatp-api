using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Configuration;
using System;

namespace SFA.DAS.Roatp.Jobs.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
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

        return services;
    }
}
