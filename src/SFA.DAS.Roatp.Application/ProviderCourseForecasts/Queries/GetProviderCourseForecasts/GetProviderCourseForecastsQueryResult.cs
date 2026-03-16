using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryResult
{
    public string LarsCode { get; set; }
    public string CourseName { get; set; }
    public int CourseLevel { get; set; }
    public IEnumerable<ProviderCourseForecastModel> Forecasts { get; set; } = [];
}
