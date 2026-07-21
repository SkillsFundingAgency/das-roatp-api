using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class Shortlist
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Ukprn { get; set; }
    public string LarsCode { get; set; }
    public string LocationName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime CreatedDate { get; set; }
}
