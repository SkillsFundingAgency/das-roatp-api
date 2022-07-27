using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Location.Queries.GetProviderLocationDetails
{
    public class GetProviderLocationDetailsQueryHandler : IRequestHandler<GetProviderLocationDetailsQuery, GetProviderLocationDetailsQueryResult>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;

        public GetProviderLocationDetailsQueryHandler(IProviderLocationsReadRepository providerLocationsReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
        }

        public async Task<GetProviderLocationDetailsQueryResult> Handle(GetProviderLocationDetailsQuery request, CancellationToken cancellationToken)
        {
            var location = await _providerLocationsReadRepository.GetProviderLocation(request.Ukprn, request.Id);
            var result = new GetProviderLocationDetailsQueryResult
            {
                Location = (ProviderLocationModel)location
            };
            return result;
        }
    }
}
