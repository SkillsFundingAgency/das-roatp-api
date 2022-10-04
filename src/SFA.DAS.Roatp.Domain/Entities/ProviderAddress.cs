using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderAddress
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? AddressUpdateDate { get; set; }
        public DateTime? CoordinatesUpdateDate { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
