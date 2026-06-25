using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities;

public class RestrictedCourse
{
    public int Id { get; set; }
    public string LarsCode { get; set; }

    public virtual ICollection<ProviderAllowedCourse> ProviderAllowedCourses { get; set; }
        = new List<ProviderAllowedCourse>();
}