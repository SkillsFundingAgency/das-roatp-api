using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQueryHandler : IRequestHandler<GetProviderSummaryQuery, ValidatedResponse<GetProviderSummaryQueryResult>>
    {
        private readonly IProviderRegistrationDetailsReadRepository _providersRegistrationDetailReadRepository;
        private readonly ILogger<GetProviderSummaryQueryHandler> _logger;

        public GetProviderSummaryQueryHandler(IProviderRegistrationDetailsReadRepository providersRegistrationDetailReadRepository, ILogger<GetProviderSummaryQueryHandler> logger)
        {
            _providersRegistrationDetailReadRepository = providersRegistrationDetailReadRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<GetProviderSummaryQueryResult>> Handle(GetProviderSummaryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider summary for ukprn [{ukprn}]", request.Ukprn);
            var provider = await _providersRegistrationDetailReadRepository.GetProviderRegistrationDetail(request.Ukprn);
            return new ValidatedResponse<GetProviderSummaryQueryResult>(new GetProviderSummaryQueryResult
            {
                ProviderSummary = provider
            });
        }
    }
}
