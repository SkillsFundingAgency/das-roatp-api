using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class GetCourseTrainingProvidersCountQueryResult
{
    public GetCourseTrainingProvidersCountQueryResult(List<CourseTrainingProviderCountModel> courses)
    {
        Courses = courses;
    }

    public List<CourseTrainingProviderCountModel> Courses { get; set; }
}
