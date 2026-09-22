using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common;

public interface IProviderCourseTypeRestriction
{
    int Ukprn { get; set; }
    CourseType CourseType { get; set; }
}
