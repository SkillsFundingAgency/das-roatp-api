using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
public class ProviderCourseTypeModel
{
    public string CourseType { get; set; }

    public static implicit operator ProviderCourseTypeModel(ProviderCourseType source) =>
        new()
        {
            CourseType = source.CourseType
        };
}
