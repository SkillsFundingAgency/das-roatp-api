using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;

public record GetRegisteredProviderQuery(int Ukprn) : IRequest<ValidatedResponse<GetRegisteredProviderQueryResult>>;
