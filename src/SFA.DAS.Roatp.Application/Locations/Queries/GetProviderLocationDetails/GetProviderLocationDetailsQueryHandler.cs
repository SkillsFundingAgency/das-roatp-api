using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQueryHandler : IRequestHandler<GetProviderLocationDetailsQuery, ValidatedResponse<ProviderLocationModel>>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;

        public GetProviderLocationDetailsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
        }

        public async Task< ValidatedResponse<ProviderLocationModel>> Handle(GetProviderLocationDetailsQuery request, CancellationToken cancellationToken)
        {
            var location = await _providerLocationsReadRepository.GetProviderLocation(request.Ukprn, request.Id);
            var result  = (ProviderLocationModel)location;
            return new ValidatedResponse<ProviderLocationModel>( result);
        }
    }
}
