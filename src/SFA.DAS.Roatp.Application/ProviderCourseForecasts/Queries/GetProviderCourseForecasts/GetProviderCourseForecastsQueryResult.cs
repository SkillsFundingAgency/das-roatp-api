using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryResult
{
    public IEnumerable<ProviderCourseForecastModel> Forecasts { get; set; } = [];
}
