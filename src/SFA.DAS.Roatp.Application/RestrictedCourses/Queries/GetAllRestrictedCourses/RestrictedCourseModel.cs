using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class RestrictedCourseModel
{
    public string LarsCode { get; set; }
    public string CourseName { get; set; }

    public static implicit operator RestrictedCourseModel(Standard source) => new()
    {
        LarsCode = source.LarsCode,
        CourseName = $"{source.Title} (Level {source.Level})"
    };
}