using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProvider
{
    public class GetProviderQuery : IRequest<GetProviderQueryResult>,  IUkprn
    {
        public int Ukprn { get; }

        public GetProviderQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
