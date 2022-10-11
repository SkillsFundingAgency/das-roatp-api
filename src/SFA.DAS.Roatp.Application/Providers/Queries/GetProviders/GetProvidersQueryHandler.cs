using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders
{
    public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, GetProvidersQueryResult>
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly ILogger<GetProvidersQueryHandler> _logger;

        public GetProvidersQueryHandler(IProvidersReadRepository providersReadRepository,  ILogger<GetProvidersQueryHandler> logger)
        {
            _providersReadRepository = providersReadRepository;
            _logger = logger;
        }
        
        public async Task<GetProvidersQueryResult> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting providers summary");
            var providers = await _providersReadRepository.GetAllProviders();
            var providersSummary = providers.Select(p => (ProviderSummary)p).ToList();
            return new GetProvidersQueryResult
            {
                RegisteredProviders = providersSummary
            };
        }
    }
}
