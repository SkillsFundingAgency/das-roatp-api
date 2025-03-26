using System;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

public record GetShortlistsForUserQuery(Guid UserId) : IRequest<ValidatedResponse<GetShortlistsForUserQueryResult>>;
