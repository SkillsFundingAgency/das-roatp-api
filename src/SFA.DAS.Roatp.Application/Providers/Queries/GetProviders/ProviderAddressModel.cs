using SFA.DAS.Roatp.Domain.Entities;
using System;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class ProviderAddressModel
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? AddressUpdateDate { get; set; }
        public DateTime? CoordinatesUpdateDate { get; set; }

        public static implicit operator ProviderAddressModel(ProviderAddress source)
        {
            if(source == null)
                return null;

            return new ProviderAddressModel
            {
                Id = source.Id,
                ProviderId = source.ProviderId,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                Town = source.Town,
                Postcode = source.Postcode,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                AddressUpdateDate = source.AddressUpdateDate,
                CoordinatesUpdateDate = source.CoordinatesUpdateDate,
            };
        }

    }
}