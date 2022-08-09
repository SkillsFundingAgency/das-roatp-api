using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations
{
    public class GetProviderCourseLocationsQueryHandler : IRequestHandler<GetProviderCourseLocationsQuery, GetProviderCourseLocationsQueryResult>
    {
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;

        public GetProviderCourseLocationsQueryHandler(IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
        }

        public async Task<GetProviderCourseLocationsQueryResult> Handle(GetProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode);
            var result = new GetProviderCourseLocationsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations.Select(x => (ProviderCourseLocationModel)x).ToList()
            };

            return result;
        }
    }
}
