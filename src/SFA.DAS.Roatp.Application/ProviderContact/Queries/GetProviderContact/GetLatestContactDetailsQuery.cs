using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;

public class GetLatestProviderContactQuery : IRequest<ValidatedResponse<GetLatestProviderContactQueryResult>>, IUkprn
{
    public int Ukprn { get; }

    public GetLatestProviderContactQuery(int ukprn)
    {
        Ukprn = ukprn;
    }
}
