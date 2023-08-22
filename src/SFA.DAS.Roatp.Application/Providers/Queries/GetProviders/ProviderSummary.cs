using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class ProviderSummary
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactUrl { get; set; }
        public int ProviderTypeId { get; set; }
        public int StatusId { get; set; }
        public ProviderAddressModel Address { get; set; } = new ProviderAddressModel();

        public static implicit operator ProviderSummary(ProviderRegistrationDetail source) =>
            source == null ? null : new ProviderSummary
            {
                Ukprn = source.Ukprn,
                Name = source.LegalName,
                ProviderTypeId = source.ProviderTypeId,
                TradingName = source.Provider?.TradingName,
                Email = source.Provider?.Email,
                Phone = source.Provider?.Phone,
                ContactUrl = source.Provider?.Website,
                StatusId = source.StatusId,
                Address = source
            };
    }
}