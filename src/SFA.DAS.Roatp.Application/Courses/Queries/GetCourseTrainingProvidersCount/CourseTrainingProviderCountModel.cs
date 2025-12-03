using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

public sealed class CourseTrainingProviderCountModel
{
    public int LarsCode { get; set; }
    public int ProvidersCount { get; set; }
    public int TotalProvidersCount { get; set; }

    public static implicit operator CourseTrainingProviderCountModel(CourseInformation source) => new()
    {
        LarsCode = int.TryParse(source.LarsCode, out var l) ? l : 0,
        ProvidersCount = source.ProvidersCount,
        TotalProvidersCount = source.TotalProvidersCount
    };
}
