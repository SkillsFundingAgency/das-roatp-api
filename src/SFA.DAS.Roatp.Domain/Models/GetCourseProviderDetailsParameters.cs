using System;

namespace SFA.DAS.Roatp.Domain.Models;

public sealed class GetCourseProviderDetailsParameters
{
    public string LarsCode { get; set; }
    public long Ukprn { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string LocationName { get; set; }
    public Guid ShortlistUserId { get; set; }
}
