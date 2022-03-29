﻿using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderCourseLocation
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set;}
        public int ProviderCourseId { get; set; }
        public int? ProviderLocationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool? HasNationalDeliveryOption { get; set; }
        public bool? OffersPortableFlexiJob { get; set; }
        public virtual ProviderCourse Course { get; set; }
        public virtual ProviderLocation Location { get; set; }
    }
}
