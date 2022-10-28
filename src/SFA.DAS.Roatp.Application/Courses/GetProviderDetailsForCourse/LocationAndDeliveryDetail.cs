using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Courses.GetProviderDetailsForCourse;

public class LocationAndDeliveryDetail
{
    public string DeliveryModes
    {
        get
        {
            var deliveryModes = new List<string>();
            if (BlockRelease)
                deliveryModes.Add("BlockRelease");
            if (DayRelease)
                deliveryModes.Add("DayRelease");
            if (LocationType == LocationType.National || LocationType == LocationType.Regional)
                deliveryModes.Add("100PercentEmployer");

            return string.Join("|", deliveryModes);
        }
    }

    public LocationType LocationType { get; set; }
    public bool BlockRelease { get; set; }
    public bool DayRelease { get; set; }
    public string RegionName { get; set; }
    public string SubRegionName { get; set; }
    public LocationAddress Address { get; set; }
    public int? LocationId { get; set; }
    public double Distance { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public static implicit operator LocationAndDeliveryDetail(ProviderLocationDetailsWithDistance source)
    {
        return new LocationAndDeliveryDetail
        {
            LocationType = source.LocationType,
            BlockRelease = source.HasBlockReleaseDeliveryOption ?? false,
            DayRelease = source.HasDayReleaseDeliveryOption ?? false,
            RegionName = source.RegionName,
            SubRegionName = source.SubregionName,
            Distance = source.Distance,
            Latitude = source.Latitude,
            Longitude = source.Longitude,
            LocationId = source.LocationId,
            Address = new LocationAddress
            {
                Address1 = source.AddressLine1,
                Address2 = source.Addressline2,
                Town = source.Town,
                Postcode = source.Postcode
            }
        };
    }
}