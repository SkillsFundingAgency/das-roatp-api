using System;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationModel
    {
        public int Id { get; set; }
        public Guid NavigationId { get; set; }
        public int ProviderCourseId { get; set; }
        public int? ProviderLocationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool IsImported { get; set; }
        public string LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public string RegionName { get; set; }

        public static implicit operator ProviderCourseLocationModel(ProviderCourseLocation source) =>
            new ProviderCourseLocationModel
            {
                Id = source.Id,
                NavigationId = source.NavigationId,
                ProviderCourseId = source.ProviderCourseId,
                ProviderLocationId = source.ProviderLocationId,
                HasDayReleaseDeliveryOption = source.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = source.HasBlockReleaseDeliveryOption,
                IsImported = source.IsImported,
                LocationName = source.Location.LocationType==LocationType.Provider ? source.Location.LocationName: source.Location.Region?.SubregionName,
                LocationType = source.Location.LocationType,
                RegionName = source.Location.Region?.RegionName
            };
    }
}
