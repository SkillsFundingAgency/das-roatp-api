using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class ProviderAddressModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }

        public static implicit operator ProviderAddressModel(ProviderRegistrationDetail source) =>
            new ProviderAddressModel
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                Town = source.Town,
                Postcode = source.Postcode,
            };

        public static implicit operator ProviderAddressModel(ProviderAddress source)
        {
            if (source == null)
                return null;

            return new ProviderAddressModel
            {
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                AddressLine3 = source.AddressLine3,
                AddressLine4 = source.AddressLine4,
                Town = source.Town,
                Postcode = source.Postcode,
            };
        }
    }
}