using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;

public class GetRegisteredProviderQueryHandler(IProviderRegistrationDetailsReadRepository _providersRegistrationDetailReadRepository) : IRequestHandler<GetRegisteredProviderQuery, ValidatedResponse<GetRegisteredProviderQueryResult>>
{
    public async Task<ValidatedResponse<GetRegisteredProviderQueryResult>> Handle(GetRegisteredProviderQuery request, CancellationToken cancellationToken)
    {
        ProviderRegistrationDetail providerSummary = await _providersRegistrationDetailReadRepository.GetProviderRegistrationDetail(request.Ukprn, cancellationToken);

        return new ValidatedResponse<GetRegisteredProviderQueryResult>(providerSummary);
    }
}
