using System;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class ProviderCourseLocationCommandModel
    {
        public Guid ProviderLocationId { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
