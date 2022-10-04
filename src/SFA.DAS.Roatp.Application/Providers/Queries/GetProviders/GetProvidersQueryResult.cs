namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class GetProvidersQueryResult
    {
        public int Ukprn { get; set; }
        public string Name { get; set; }
        public string TradingName { get; set; }
        public string ContactUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ProviderAddress Address { get; set; }

        public static implicit operator GetProvidersQueryResult(Domain.Entities.Provider source) =>
            new GetProvidersQueryResult
            {
                Ukprn = source.Ukprn,
                Name = source.LegalName,
                TradingName = source.TradingName,
                Email = source.Email,
                Phone = source.Phone,
                ContactUrl = source.Website,
                Address = source.
            };
    }
}
