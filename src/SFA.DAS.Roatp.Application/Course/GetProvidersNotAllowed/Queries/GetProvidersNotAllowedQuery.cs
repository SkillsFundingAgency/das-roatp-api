using MediatR;

namespace SFA.DAS.Roatp.Application.Course.GetProvidersNotAllowed.Queries;

public record GetProvidersNotAllowedQuery(string LarsCode) : IRequest<GetProvidersNotAllowedQueryResult>;