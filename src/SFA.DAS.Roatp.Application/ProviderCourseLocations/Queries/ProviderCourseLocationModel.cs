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
        public decimal Radius { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool? OffersPortableFlexiJob { get; set; }
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
                Radius = source.Radius,
                HasDayReleaseDeliveryOption = source.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = source.HasBlockReleaseDeliveryOption,
                OffersPortableFlexiJob = source.OffersPortableFlexiJob,
                IsImported = source.IsImported,
                LocationName = source.Location.LocationName,
                LocationType = source.Location.LocationType,
                RegionName = source.Location.Region?.RegionName
            };
    }
}
