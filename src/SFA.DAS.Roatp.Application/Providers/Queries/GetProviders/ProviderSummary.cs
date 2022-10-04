using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class ProviderSummary
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get ; set ; }
        public string ContactUrl { get ; set ; }
        public ProviderAddress Address { get; set; }

        public static implicit operator ProviderSummary(Provider source)
        {
            if (source == null)
            {
                return null;
            }
            return new ProviderSummary
            {
                Ukprn = source.Ukprn,
                Name = source.LegalName,
                TradingName = source.TradingName,
                Email = source.Email,
                Phone = source.Phone,
                ContactUrl = source.Website,
                Address = source.Address
            };
        }
    }
}