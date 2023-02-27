using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations
{
    public class GetProviderLocationsQueryHandler : IRequestHandler<GetProviderLocationsQuery, ValidatedResponse<List<ProviderLocationModel>>>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;

        public GetProviderLocationsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
        }

        public async Task<ValidatedResponse<List<ProviderLocationModel>>> Handle(GetProviderLocationsQuery request, CancellationToken cancellationToken)
        {
            var locations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
            var result  = locations.Select(x => (ProviderLocationModel)x).ToList();
            return new ValidatedResponse<List<ProviderLocationModel>>(result);
        }
    }
}
