using System;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class ProviderCourseLocationModel
    {
        public Guid NavigationId { get; set; }
        public int ProviderCourseId { get; set; }
        public int? ProviderLocationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool IsImported { get; set; }
        public string LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public string RegionName { get; set; }
        public string SubregionName { get; set; }
        public int? RegionId { get; set; }

        public static implicit operator ProviderCourseLocationModel(ProviderCourseLocation source) =>
            new ProviderCourseLocationModel
            {
                NavigationId = source.NavigationId,
                ProviderCourseId = source.ProviderCourseId,
                ProviderLocationId = source.ProviderLocationId,
                HasDayReleaseDeliveryOption = source.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = source.HasBlockReleaseDeliveryOption,
                IsImported = source.IsImported,
                LocationName = source.Location.LocationName,
                LocationType = source.Location.LocationType,
                RegionName = source.Location.Region?.RegionName,
                SubregionName = source.Location.Region?.SubregionName,
                RegionId = source.Location.Region?.Id
            };
    }
}
