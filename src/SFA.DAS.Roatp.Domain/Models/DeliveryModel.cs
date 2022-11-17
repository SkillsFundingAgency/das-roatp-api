using System.ComponentModel;

namespace SFA.DAS.Roatp.Domain.Models;

public class DeliveryModel
{
    public LocationType LocationType { get; set; }
    public bool? DayRelease { get; set; }
    public bool? BlockRelease { get; set; }
    public double? DistanceInMiles { get; set; }
}

