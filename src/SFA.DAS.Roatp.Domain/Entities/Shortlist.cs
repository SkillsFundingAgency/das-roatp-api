using System;

namespace SFA.DAS.Roatp.Domain.Entities;

public class Shortlist
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Ukprn { get; set; }
    public int LarsCode { get; set; }
    public string LocationDescription { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime CreatedDate { get; set; }
}
