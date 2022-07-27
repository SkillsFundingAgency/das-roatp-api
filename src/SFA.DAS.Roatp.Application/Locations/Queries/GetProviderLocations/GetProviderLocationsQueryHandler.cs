using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class GetProviderLocationsQueryHandler : IRequestHandler<GetProviderLocationsQuery, GetProviderLocationsQueryResult>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;

        public GetProviderLocationsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
        }

        public async Task<GetProviderLocationsQueryResult> Handle(GetProviderLocationsQuery request, CancellationToken cancellationToken)
        {
            var locations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
            var result = new GetProviderLocationsQueryResult
            {
                Locations = locations.Select(x => (ProviderLocationModel)x).ToList()
            };
            return result;
        }
    }
}
