using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderCourse
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ProviderId { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }

        public Provider Provider { get; set; }
        public List<ProviderCourseLocation> Locations { get; set; } = new();
        public List<ProviderCourseVersion> Versions { get; set; } = new();
    }
}
