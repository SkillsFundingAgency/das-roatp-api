using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class ProviderLocationModel
    {
        public int ProviderLocationId { get; set; }
        public Guid NavigationId { get; set; }
        public int? RegionId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool IsImported { get; set; }
        public LocationType LocationType { get; set; }
        public List<LocationStandardModel> Standards { get; set; }

        public static implicit operator ProviderLocationModel(ProviderLocation source) =>
            new ProviderLocationModel
            {
                ProviderLocationId = source.Id,
                NavigationId = source.NavigationId,
                RegionId = source.RegionId,
                LocationName = source.LocationName,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                Town = source.Town,
                Postcode = source.Postcode,
                County = source.County,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                IsImported = source.IsImported,
                LocationType = source.LocationType,
            };
    }
}
