using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvider
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, ValidatedResponse<GetProviderQueryResult>>
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly IProviderRegistrationDetailsReadRepository _providerRegistrationDetailsReadRepository;
        private readonly ILogger<GetProviderQueryHandler> _logger;

        public GetProviderQueryHandler(IProvidersReadRepository providersReadRepository, ILogger<GetProviderQueryHandler> logger, IProviderRegistrationDetailsReadRepository providerRegistrationDetailsReadRepository)
        {
            _providersReadRepository = providersReadRepository;
            _logger = logger;
            _providerRegistrationDetailsReadRepository = providerRegistrationDetailsReadRepository;
        }

        public async Task<ValidatedResponse<GetProviderQueryResult>> Handle(GetProviderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting provider for ukprn [{ukprn}]", request.Ukprn);
            var provider = await _providersReadRepository.GetByUkprn(request.Ukprn);
            var getProviderQueryResult = (GetProviderQueryResult)provider;
            var providerRegistrationDetail = await _providerRegistrationDetailsReadRepository.GetProviderRegistrationDetail(request.Ukprn);
        
            if (providerRegistrationDetail != null)
            {
                getProviderQueryResult.ProviderType = (ProviderType)providerRegistrationDetail.ProviderTypeId;
                getProviderQueryResult.ProviderStatusType = (ProviderStatusType)providerRegistrationDetail.StatusId;
                getProviderQueryResult.ProviderStatusUpdatedDate = providerRegistrationDetail.StatusDate;
            }
            return new ValidatedResponse<GetProviderQueryResult>(getProviderQueryResult);
        }
    }
}
