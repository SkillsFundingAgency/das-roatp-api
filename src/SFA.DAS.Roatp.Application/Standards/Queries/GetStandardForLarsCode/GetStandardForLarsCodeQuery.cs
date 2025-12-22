using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;

public class GetStandardForLarsCodeQuery : IRequest<ValidatedResponse<GetStandardForLarsCodeQueryResult>>, ILarsCode
{
    public string LarsCode { get; }
    public GetStandardForLarsCodeQuery(string larsCode)
    {
        LarsCode = larsCode;
    }
}