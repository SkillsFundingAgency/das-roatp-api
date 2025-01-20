using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;

public sealed class CourseModel
{
    public int LarsCode { get; set; }
    public int ProvidersCount { get; set; }
    public int TotalProvidersCount { get; set; }

    public static implicit operator CourseModel(CourseInformation source) => new()
    {
        LarsCode = source.LarsCode,
        ProvidersCount = source.ProvidersCount,
        TotalProvidersCount = source.TotalProvidersCount
    };
}
