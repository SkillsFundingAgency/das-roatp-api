using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

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
            _logger.LogInformation("Getting provider for ukprn [{ukprn}]", request.Ukprn);
            var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
            return provider;
        }
    }
}
