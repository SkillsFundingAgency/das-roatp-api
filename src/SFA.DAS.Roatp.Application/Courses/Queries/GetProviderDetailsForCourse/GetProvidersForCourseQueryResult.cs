using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class GetProvidersForCourseQueryResult
{
    public int LarsCode { get; set; }
    public string CourseTitle { get; set; }
    public int Level { get; set; }
    public List<ProviderDetails> Providers { get; set; }
}