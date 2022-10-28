namespace SFA.DAS.Roatp.Domain.Entities;

public class ProviderLocationDetailsWithDistance
{
    public int Ukprn { get; set; }
    public int LarsCode { get; set; }
    public string LocationName { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Phone { get; set; }
    public LocationType LocationType { get; set; }
    public int? LocationId { get; set; }
      
    public bool? HasDayReleaseDeliveryOption { get; set; }
    public bool? HasBlockReleaseDeliveryOption { get; set; }

    public string AddressLine1 { get; set; }
    public string Addressline2 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
    public string RegionName { get; set; }
    public string SubregionName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double Distance { get; set; }
}