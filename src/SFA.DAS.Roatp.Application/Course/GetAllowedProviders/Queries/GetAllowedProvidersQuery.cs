using MediatR;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;

public record GetAllowedProvidersQuery(string LarsCode) : IRequest<ValidatedResponse<GetAllowedProvidersQueryResult>>, ILarsCode;
