using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQueryHandler : IRequestHandler<GetProviderSummaryQuery, ValidatedResponse<GetProviderSummaryQueryResult>>
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly ILogger<GetProviderSummaryQueryHandler> _logger;

        public GetProviderSummaryQueryHandler(IProvidersReadRepository providersReadRepository,  ILogger<GetProviderSummaryQueryHandler> logger)
        {
            _providersReadRepository = providersReadRepository;
            _logger = logger;
        }
        
        public async Task<ValidatedResponse<GetProviderSummaryQueryResult>> Handle(GetProviderSummaryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider summary for ukprn [{ukprn}]", request.Ukprn);
            var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
            return new ValidatedResponse<GetProviderSummaryQueryResult>(new GetProviderSummaryQueryResult
            {
                ProviderSummary = (ProviderSummary)provider
            });
        }
    }
}
