using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;

public class GetProviderSummaryQueryHandler : IRequestHandler<GetProviderSummaryQuery, ValidatedResponse<GetProviderSummaryQueryResult>>
{
    private readonly IProviderRegistrationDetailsReadRepository _providersRegistrationDetailReadRepository;

    public GetProviderSummaryQueryHandler(IProviderRegistrationDetailsReadRepository providersRegistrationDetailReadRepository)
    {
        _providersRegistrationDetailReadRepository = providersRegistrationDetailReadRepository;
    }

    public async Task<ValidatedResponse<GetProviderSummaryQueryResult>> Handle(GetProviderSummaryQuery request, CancellationToken cancellationToken)
    {
        ProviderSummaryModel providerSummary = await _providersRegistrationDetailReadRepository.GetProviderSummary(request.Ukprn, cancellationToken);

        if(providerSummary is null)
        {
            return new ValidatedResponse<GetProviderSummaryQueryResult>((GetProviderSummaryQueryResult)null);
        }

        return new ValidatedResponse<GetProviderSummaryQueryResult>((GetProviderSummaryQueryResult)providerSummary);
    }
}
