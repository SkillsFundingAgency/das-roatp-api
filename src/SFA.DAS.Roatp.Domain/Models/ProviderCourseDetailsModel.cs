using System.Security.Cryptography.X509Certificates;

namespace SFA.DAS.Roatp.Domain.Models
{
    public class ProviderCourseDetailsModel : ProviderCourseDetailsModelBase
    {
        public string MarketingInfo { get; set; }
    }
    public class ProviderCourseDetailsSummaryModel: ProviderCourseDetailsModelBase
    {
       
        public int ProviderId { get; set; }
    }
    public class ProviderCourseDetailsModelBase
    {
        public int Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
      
        public string StandardInfoUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string StandardContactUrl { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public double? Distance { get; set; }
    }
}