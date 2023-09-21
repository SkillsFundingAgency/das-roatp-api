using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderStatus
{
    public class GetProviderStatusQuery : IRequest<ValidatedResponse<GetProviderStatusResult>>, IUkprn
    {
        public int Ukprn { get; }

        public GetProviderStatusQuery(int ukprn)
        {
            Ukprn = ukprn;
        }
    }
}
