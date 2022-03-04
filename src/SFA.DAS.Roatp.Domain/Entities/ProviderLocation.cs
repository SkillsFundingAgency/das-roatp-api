using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderLocation
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public int ProviderId { get; set; }
        public string LocationName { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Radius { get; set; }

        public virtual Provider Provider { get; set; }

    }
}
