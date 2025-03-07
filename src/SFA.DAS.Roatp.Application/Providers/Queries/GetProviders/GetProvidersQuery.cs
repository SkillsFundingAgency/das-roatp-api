using MediatR;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;

public class GetProvidersQuery : IRequest<GetProvidersQueryResult>
{
    public bool Live { get; set; } = false;
}
