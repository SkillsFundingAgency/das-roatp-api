using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Common;

public class CourseTypeUkprnValidationObject : IUkprn, ICourseType
{
    public int Ukprn { get; set; }
    public CourseType CourseType { get; set; }
}