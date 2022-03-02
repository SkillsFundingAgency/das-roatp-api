using System.Collections.Generic;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseModel
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public List<string> DeliveryModels { get; set; } = new List<string>();
    }
}
