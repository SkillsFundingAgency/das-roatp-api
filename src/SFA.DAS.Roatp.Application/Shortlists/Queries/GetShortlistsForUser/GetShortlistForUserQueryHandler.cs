using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistForUserQueryHandler : IRequestHandler<GetShortlistsForUserQuery, ValidatedResponse<GetShortlistsForUserQueryResult>>
{
    public async Task<ValidatedResponse<GetShortlistsForUserQueryResult>> Handle(GetShortlistsForUserQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new ValidatedResponse<GetShortlistsForUserQueryResult>(new GetShortlistsForUserQueryResult() { UserId = request.UserId }));
    }
}
