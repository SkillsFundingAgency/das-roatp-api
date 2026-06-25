using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities;

public class RestrictedCourse
{
    public int Id { get; set; }
    public string LarsCode { get; set; }

    public virtual List<ProviderAllowedCourse> ProviderAllowedCourses { get; set; }
        = new List<ProviderAllowedCourse>();
}