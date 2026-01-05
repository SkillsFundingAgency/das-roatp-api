using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class CourseTrainingProviderCountModel
{
    public string LarsCode { get; set; }
    public int ProvidersCount { get; set; }
    public int TotalProvidersCount { get; set; }

    public static implicit operator CourseTrainingProviderCountModel(CourseInformation source) => new()
    {
        LarsCode = source.LarsCode,
        ProvidersCount = source.ProvidersCount,
        TotalProvidersCount = source.TotalProvidersCount
    };
}
