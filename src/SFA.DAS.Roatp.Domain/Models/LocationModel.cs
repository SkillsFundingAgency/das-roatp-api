namespace SFA.DAS.Roatp.Domain.Models;

public sealed class LocationModel
{
    public long Ordering { get; set; }
    public bool AtEmployer { get; set; }
    public bool BlockRelease { get; set; }
    public bool DayRelease { get; set; }
    public int LocationType { get; set; }
    public string CourseLocation { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
    public double CourseDistance { get; set; }

    public static implicit operator LocationModel(CourseProviderDetailsModel source)
    {
        return new LocationModel
        {
            Ordering = source.Ordering,
            AtEmployer = source.AtEmployer,
            BlockRelease = source.BlockRelease,
            DayRelease = source.DayRelease,
            LocationType = source.LocationType,
            CourseLocation = source.CourseLocation,
            AddressLine1 = source.AddressLine1,
            AddressLine2 = source.AddressLine2,
            Town = source.Town,
            County = source.County,
            Postcode = source.Postcode,
            CourseDistance = source.CourseDistance
        };
    }
}
