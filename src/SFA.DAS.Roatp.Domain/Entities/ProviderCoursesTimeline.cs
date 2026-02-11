using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderCoursesTimeline
{
    public int ProviderId { get; set; }
    public string LarsCode { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public virtual Provider Provider { get; set; }
    public virtual Standard Standard { get; set; }
}
