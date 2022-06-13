using System.Collections.Generic;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProviderCourseLocation
    {
        public int Id { get; set; }
        public int? Radius { get; set; } // might not be needed
        public List<string> DeliveryModes { get; set; }
    }
}
