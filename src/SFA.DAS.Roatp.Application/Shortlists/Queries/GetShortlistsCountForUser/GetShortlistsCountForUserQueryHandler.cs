using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;

public class GetShortlistsCountForUserQueryHandler(IShortlistWriteRepository shortlistWriteRepository) : IRequestHandler<GetShortlistsCountForUserQuery, ValidatedResponse<GetShortlistsCountForUserQueryResult>>
{
    public async Task<ValidatedResponse<GetShortlistsCountForUserQueryResult>> Handle(GetShortlistsCountForUserQuery request, CancellationToken cancellationToken)
    {
        var count = await shortlistWriteRepository.GetShortlistCount(request.UserId, cancellationToken);
        return new ValidatedResponse<GetShortlistsCountForUserQueryResult>(new GetShortlistsCountForUserQueryResult(count));
    }
}
