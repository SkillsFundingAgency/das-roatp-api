using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary
{
    public class GetProviderSummaryQuery : IRequest<GetProviderSummaryQueryResult>,  IUkprn
    {
        public int Ukprn { get; }

        public GetProviderSummaryQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
