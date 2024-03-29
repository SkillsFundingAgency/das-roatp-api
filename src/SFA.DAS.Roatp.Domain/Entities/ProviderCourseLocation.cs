﻿using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderCourseLocation
    {
        public int Id { get; set; }
        public Guid NavigationId { get; set; }
        public int ProviderCourseId { get; set; }
        public int ProviderLocationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool IsImported { get; set; } = false;
        public virtual ProviderCourse ProviderCourse { get; set; }
        public virtual ProviderLocation Location { get; set; }
    }
}
