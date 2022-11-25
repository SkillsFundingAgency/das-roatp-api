using System;

namespace SFA.DAS.Roatp.Api.Models
{
    public class AddProviderCourseLocationModel
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public Guid LocationNavigationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
    }
}
