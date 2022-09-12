using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvider
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, GetProviderQueryResult>
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly ILogger<GetProviderQueryHandler> _logger;

        public GetProviderQueryHandler(IProvidersReadRepository providersReadRepository,  ILogger<GetProviderQueryHandler> logger)
        {
            _providersReadRepository = providersReadRepository;
            _logger = logger;
        }
        
        public async Task<GetProviderQueryResult> Handle(GetProviderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider for ukprn [{ukprn}]", request.Ukprn);
            var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
            return provider;
        }
    }
}
