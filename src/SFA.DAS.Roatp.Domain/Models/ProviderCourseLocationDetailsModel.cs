﻿namespace SFA.DAS.Roatp.Domain.Models;

public class ProviderCourseLocationDetailsModel
{
    public LocationType LocationType { get; set; }
    public bool? HasDayReleaseDeliveryOption { get; set; }
    public bool? HasBlockReleaseDeliveryOption { get; set; }

    public double? Distance { get; set; }
    public int ProviderId { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
}