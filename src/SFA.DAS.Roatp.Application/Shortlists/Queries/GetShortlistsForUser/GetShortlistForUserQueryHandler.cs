using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistForUserQueryHandler(IShortlistsRepository _shortlistsRepository) : IRequestHandler<GetShortlistsForUserQuery, ValidatedResponse<GetShortlistsForUserQueryResult>>
{
    public async Task<ValidatedResponse<GetShortlistsForUserQueryResult>> Handle(GetShortlistsForUserQuery request, CancellationToken cancellationToken)
    {
        string jsonResult = await _shortlistsRepository.GetShortlistsForUser(request.UserId, cancellationToken);

        var result = JsonSerializer.Deserialize<GetShortlistsForUserQueryResult>(jsonResult, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return await Task.FromResult(new ValidatedResponse<GetShortlistsForUserQueryResult>(result));
    }
}
