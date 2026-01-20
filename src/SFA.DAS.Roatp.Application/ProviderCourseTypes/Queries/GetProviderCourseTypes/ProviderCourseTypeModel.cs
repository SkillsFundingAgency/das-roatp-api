using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;

public class ProviderCourseTypeModel
{
    public CourseType CourseType { get; set; }

    public static implicit operator ProviderCourseTypeModel(ProviderCourseType source) =>
        new()
        {
            CourseType = source.CourseType
        };
}
