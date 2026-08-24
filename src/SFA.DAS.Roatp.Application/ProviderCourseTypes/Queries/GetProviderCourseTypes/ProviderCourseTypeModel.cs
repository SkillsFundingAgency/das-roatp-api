using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;

public class ProviderCourseTypeModel
{
    public int CourseTypeId { get; set; }
    public CourseType CourseType { get; set; }
    public bool IsRestricted { get; set; }
    public int? RestrictedCount { get; set; }
    public int? AllowedCount { get; set; }

    public static implicit operator ProviderCourseTypeModel(ProviderCourseType source) =>
        new()
        {
            CourseTypeId = source.Id,
            CourseType = source.CourseType,
            IsRestricted = source.IsRestrictedProvider,
        };
}
