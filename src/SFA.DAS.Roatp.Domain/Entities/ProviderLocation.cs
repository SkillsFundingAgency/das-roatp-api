﻿using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderLocation
    {
        public int Id { get; set; }
        public Guid NavigationId { get; set; }
        public int ProviderId { get; set; }
        public int? RegionId { get; set; }
        public int? ImportedLocationId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsImported { get; set; } = false;
        public LocationType LocationType { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Region Region { get; set; }
        public virtual List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();
        public static ProviderLocation CreateNationalLocation(int providerId) => new ProviderLocation
        {
            NavigationId = Guid.NewGuid(),
            ProviderId = providerId,
            Latitude = NationalLatLong.NationalLatitude,
            Longitude = NationalLatLong.NationalLongitude,
            LocationType = LocationType.National,
            IsImported = false
        };
        public static ProviderLocation CreateRegionalLocation(int providerId, Region region) => new ProviderLocation
        {
            NavigationId = Guid.NewGuid(),
            ProviderId = providerId,
            RegionId = region.Id,
            Latitude = region.Latitude,
            Longitude = region.Longitude,
            LocationType = LocationType.Regional,
            IsImported = false
        };
    }
}
