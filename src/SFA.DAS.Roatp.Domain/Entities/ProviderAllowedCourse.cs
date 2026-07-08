using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderAllowedCourse
{
    public int Id { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public DateTime? LastDateStarts { get; set; }

    public virtual Standard Standard { get; set; }
    public virtual Provider Provider { get; set; }
    public virtual List<ProviderCourse> ProviderCourses { get; set; }
}
