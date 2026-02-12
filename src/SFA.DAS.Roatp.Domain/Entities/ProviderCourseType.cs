using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderCourseType
{
    public int Id { get; set; }
    public int Ukprn { get; set; }
    public CourseType CourseType { get; set; }
    public virtual Provider Provider { get; set; }
    public virtual ProviderRegistrationDetail ProviderRegistrationDetail { get; set; }
}
