using System;

namespace SFA.DAS.Roatp.Domain.Models;

public sealed class GetCourseProviderDetailsParameters
{
    public string LarsCode { get; set; }
    public long Ukprn { get; set; }
    public decimal? Lon { get; set; }
    public decimal? Lat { get; set; }
    public string Location { get; set; }
    public Guid ShortlistUserId { get; set; }
}
