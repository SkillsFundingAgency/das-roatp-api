using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderCourse
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int LarsCode { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsImported { get; set; } = false;
        public bool HasPortableFlexiJobOption { get; set; }
        public virtual Provider Provider { get; set; }
        public virtual Standard Standard { get; set; }
        public virtual List<ProviderCourseLocation> Locations { get; set; } = new List<ProviderCourseLocation>();
        public virtual List<ProviderCourseVersion> Versions { get; set; } = new List<ProviderCourseVersion>();
    }
}
