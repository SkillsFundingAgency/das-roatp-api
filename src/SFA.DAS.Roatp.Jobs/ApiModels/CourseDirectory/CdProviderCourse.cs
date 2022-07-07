using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProviderCourse
    {
        public int StandardCode { get; set; }
        public string StandardInfoUrl { get; set; }
        public Contact Contact { get; set; }
        public string ContactUsPhoneNumber => Contact?.Phone;
        public string ContactUsEmail => Contact?.Email;
        public string ContactUsPageUrl => Contact?.ContactUsUrl;

        public virtual List<CdProviderCourseLocation> Locations { get; set; } = new List<CdProviderCourseLocation>();
    }
}
