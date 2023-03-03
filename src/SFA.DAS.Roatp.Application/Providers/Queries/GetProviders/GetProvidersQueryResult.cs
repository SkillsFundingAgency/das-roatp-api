using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class GetProvidersQueryResult
    {
        public IEnumerable<ProviderSummary> RegisteredProviders { get; set; } = new List<ProviderSummary>();
    }
}
