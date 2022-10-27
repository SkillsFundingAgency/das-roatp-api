namespace SFA.DAS.Roatp.Domain.Entities
{

    public class ProviderDetailsWithDistance
    {
        public int Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string MarketingInfo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double Distance { get; set; }

    }
}