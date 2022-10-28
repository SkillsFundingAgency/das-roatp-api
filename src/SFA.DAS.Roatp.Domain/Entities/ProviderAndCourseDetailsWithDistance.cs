namespace SFA.DAS.Roatp.Domain.Entities
{

    public class ProviderAndCourseDetailsWithDistance
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string MarketingInfo { get; set; }
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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double Distance { get; set; }
    }
}