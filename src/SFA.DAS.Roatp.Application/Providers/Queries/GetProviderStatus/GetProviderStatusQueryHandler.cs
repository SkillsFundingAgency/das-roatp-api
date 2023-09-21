using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderStatus
{
    public class GetProviderStatusQueryHandler : IRequestHandler<GetProviderStatusQuery, ValidatedResponse<GetProviderStatusResult>>
    {
        private readonly IProviderRegistrationDetailsReadRepository _providerRegistrationDetailsReadRepository;
        private readonly ILogger<GetProviderStatusQueryHandler> _logger;

        public GetProviderStatusQueryHandler(ILogger<GetProviderStatusQueryHandler> logger, IProviderRegistrationDetailsReadRepository providerRegistrationDetailsReadRepository)
        {
            _logger = logger;
            _providerRegistrationDetailsReadRepository = providerRegistrationDetailsReadRepository;
        }

        public async Task<ValidatedResponse<GetProviderStatusResult>> Handle(GetProviderStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider status for ukprn [{ukprn}]", request.Ukprn);
            var providerRegistrationDetail = await _providerRegistrationDetailsReadRepository.GetProviderRegistrationDetail(request.Ukprn);
            if (providerRegistrationDetail != null)
            {
                // Logic to check if the provider is a valid
                // Condition 1: is the provider's profile a Main or Employer Profile.
                // Condition 2: is the provider's status Active or On-boarding.
                return new ValidatedResponse<GetProviderStatusResult>(
                    new GetProviderStatusResult
                        {
                            IsValidProvider = (ProviderType)providerRegistrationDetail.ProviderTypeId is ProviderType.Main or ProviderType.Employer
                                              && (ProviderStatusType)providerRegistrationDetail.StatusId is ProviderStatusType.Active or ProviderStatusType.Onboarding
                        });
            }
            return new ValidatedResponse<GetProviderStatusResult>(new GetProviderStatusResult());
        }
    }
}
