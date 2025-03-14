using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;

public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, GetProvidersQueryResult>
{
    private readonly IProviderRegistrationDetailsReadRepository _providersRegistrationDetailReadRepository;
    private readonly ILogger<GetProvidersQueryHandler> _logger;

    public GetProvidersQueryHandler(IProviderRegistrationDetailsReadRepository providersRegistrationDetailReadRepository, ILogger<GetProvidersQueryHandler> logger)
    {
        _providersRegistrationDetailReadRepository = providersRegistrationDetailReadRepository;
        _logger = logger;
    }

    public async Task<GetProvidersQueryResult> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting providers summary");
        var providers = 
            request.Live ? 
                await _providersRegistrationDetailReadRepository.GetActiveAndMainProviderRegistrations(cancellationToken) :
                await _providersRegistrationDetailReadRepository.GetActiveProviderRegistrations(cancellationToken);
        var providersSummary = providers.Select(p => (ProviderSummary)p);
        return new GetProvidersQueryResult
        {
            RegisteredProviders = providersSummary
        };
    }
}
