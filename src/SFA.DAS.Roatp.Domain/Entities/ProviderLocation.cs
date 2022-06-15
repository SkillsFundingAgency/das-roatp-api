﻿using System;
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
        public string SubregionName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public bool IsImported { get; set; }
        public LocationType LocationType { get; set; }

        public virtual Provider Provider { get; set; }

        public virtual Region Region { get; set; }
        public virtual List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();
    }
}
