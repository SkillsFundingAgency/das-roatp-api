using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderRegisterDetailsLookup;

public class GetProviderRegisterDetailsLookupQuery : IRequest<ValidatedResponse<GetProviderRegisterDetailsLookupQueryResult>>, IUkprn
{
    public int Ukprn { get; }

    public GetProviderRegisterDetailsLookupQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}
