using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class CourseLocationModel
{
    public LocationType LocationType { get; set; }
    public bool BlockRelease { get; set; }
    public bool DayRelease { get; set; }
    public string RegionName { get; set; }
    public string SubRegionName { get; set; }
    public LocationAddress Address { get; set; }
    public double Distance { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public static implicit operator CourseLocationModel(ProviderCourseLocationDetailsModel source)
    {
        return new CourseLocationModel
        {
            LocationType = source.LocationType,
            BlockRelease = source.HasBlockReleaseDeliveryOption ?? false,
            DayRelease = source.HasDayReleaseDeliveryOption ?? false,
            RegionName = source.RegionName,
            SubRegionName = source.SubregionName,
            Distance = source.Distance,
            Latitude = source.Latitude,
            Longitude = source.Longitude,
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