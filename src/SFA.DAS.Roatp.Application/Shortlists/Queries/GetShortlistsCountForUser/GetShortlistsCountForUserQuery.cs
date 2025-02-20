using System;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;
public record GetShortlistsCountForUserQuery(Guid UserId) : IRequest<ValidatedResponse<GetShortlistsCountForUserQueryResult>>;
