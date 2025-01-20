using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryResult
{
    public GetCoursesQueryResult(List<CourseModel> courses)
    {
        Courses = courses;
    }

    public List<CourseModel> Courses { get; set; }
}
