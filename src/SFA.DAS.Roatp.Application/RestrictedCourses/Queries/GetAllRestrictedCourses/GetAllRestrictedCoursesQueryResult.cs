using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class GetAllRestrictedCoursesQueryResult
{
    public List<RestrictedCourseModel> Courses { get; set; } = new List<RestrictedCourseModel>();
}
