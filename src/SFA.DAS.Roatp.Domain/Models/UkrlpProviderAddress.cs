namespace SFA.DAS.Roatp.Domain.Models
{
    public class UkrlpProviderAddress
    {
        public int Id { get; set; }
        public int Ukprn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public int? ProviderId { get; set; }
    }
}
