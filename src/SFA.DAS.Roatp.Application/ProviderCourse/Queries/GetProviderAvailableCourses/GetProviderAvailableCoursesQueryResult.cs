using System.Collections.Generic;
using SFA.DAS.Roatp.Application.Common.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;

public class GetProviderAvailableCoursesQueryResult
{
    public IEnumerable<CourseBasicModel> AvailableCourses { get; set; } = [];
}
