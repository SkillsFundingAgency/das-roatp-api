using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;

public class GetProviderSummaryQuery : IRequest<ValidatedResponse<GetProviderSummaryQueryResult>>,  IUkprn
{
    public int Ukprn { get; }

    public GetProviderSummaryQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}
