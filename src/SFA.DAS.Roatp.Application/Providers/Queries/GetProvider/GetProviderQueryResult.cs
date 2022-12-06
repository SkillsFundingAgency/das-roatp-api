using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvider
{
    public class GetProviderQueryResult
    {
        public int Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string MarketingInfo { get; set; }
        public bool IsImported { get; set; } = false;
        public ProviderType ProviderType { get; set; }

        public static implicit operator GetProviderQueryResult(Domain.Entities.Provider source) =>
            new GetProviderQueryResult
            {
                Ukprn = source.Ukprn,
                LegalName = source.LegalName,
                TradingName = source.TradingName,
                Email = source.Email,
                Phone = source.Phone,
                Website = source.Website,
                MarketingInfo = source.MarketingInfo,
                IsImported = source.IsImported,
            };
    }
}
