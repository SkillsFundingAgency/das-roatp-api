using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQueryResult
{
    public int LarsCode { get; set; }
    public string CourseTitle { get; set; }
    public int Level { get; set; }
    public List<ProviderSummation> Providers { get; set; }
}