namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderCourseLocationDetailsModel
{
    public LocationType LocationType { get; set; }
    public bool? HasDayReleaseDeliveryOption { get; set; }
    public bool? HasBlockReleaseDeliveryOption { get; set; }

    public double? Distance { get; set; }
    public int ProviderId { get; set; }
}