using System;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderTimelineExport
{
    public int Ukprn { get; set; }
    public int StatusId { get; set; }
    public int ProviderTypeId { get; set; }
    public CourseType? CourseType { get; set; }
    public string LarsCode { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime? LastDateStarts { get; set; }
}
