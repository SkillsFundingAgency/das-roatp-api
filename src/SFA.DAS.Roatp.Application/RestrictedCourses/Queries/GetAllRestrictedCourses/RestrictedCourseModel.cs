using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class RestrictedCourseModel
{
    public string LarsCode { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }

    public static implicit operator RestrictedCourseModel(Standard source) => new()
    {
        LarsCode = source.LarsCode,
        Title = source.Title,
        Level = source.Level
    };
}