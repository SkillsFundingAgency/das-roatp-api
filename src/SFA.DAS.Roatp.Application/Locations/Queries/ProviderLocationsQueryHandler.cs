using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries
{
    public class ProviderLocationsQueryHandler : IRequestHandler<ProviderLocationsQuery, ProviderLocationsQueryResult>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;

        public ProviderLocationsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
        }

        public async Task<ProviderLocationsQueryResult> Handle(ProviderLocationsQuery request, CancellationToken cancellationToken)
        {
            var locations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
            var result = new ProviderLocationsQueryResult();
            result.Locations = locations.Select(x => (ProviderLocationModel)x).ToList();
            return result;
        }
    }
}
