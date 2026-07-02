using MediatR;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public record GetAllowedProvidersQuery(string LarsCode) : IRequest<GetAllowedProvidersQueryResult>;
