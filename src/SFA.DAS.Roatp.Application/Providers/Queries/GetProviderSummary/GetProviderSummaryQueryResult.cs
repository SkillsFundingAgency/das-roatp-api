using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQueryResult
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ContactUrl { get; set; }
        public ProviderAddressModel Address { get; set; } = new ProviderAddressModel();

        public static implicit operator GetProviderSummaryQueryResult(ProviderRegistrationDetail source) =>
            source == null ? null : new GetProviderSummaryQueryResult
            {
                Ukprn = source.Ukprn,
                Name = source.LegalName,
                TradingName = source.Provider?.TradingName,
                Email = source.Provider?.Email,
                Phone = source.Provider?.Phone,
                ContactUrl = source.Provider?.Website,
                Address = source
            };
    }
}
