using MediatR;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.Provider.Queries.GetProvider
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
