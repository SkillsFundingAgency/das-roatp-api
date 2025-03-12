using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistForUserQueryHandler(IShortlistsRepository _shortlistsRepository) : IRequestHandler<GetShortlistsForUserQuery, ValidatedResponse<GetShortlistsForUserQueryResult>>
{
    public static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<ValidatedResponse<GetShortlistsForUserQueryResult>> Handle(GetShortlistsForUserQuery request, CancellationToken cancellationToken)
    {
        int count = await _shortlistsRepository.GetShortlistCount(request.UserId, cancellationToken);
        if (count == 0) return new ValidatedResponse<GetShortlistsForUserQueryResult>(new GetShortlistsForUserQueryResult() { UserId = request.UserId });
        string jsonResult = await _shortlistsRepository.GetShortlistsForUser(request.UserId, cancellationToken);

        var result = JsonSerializer.Deserialize<GetShortlistsForUserQueryResult>(jsonResult, SerializerOptions);

        return await Task.FromResult(new ValidatedResponse<GetShortlistsForUserQueryResult>(result));
    }
}
