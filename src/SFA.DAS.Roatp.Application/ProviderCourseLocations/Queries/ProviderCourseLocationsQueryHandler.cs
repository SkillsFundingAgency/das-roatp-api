using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQueryHandler : IRequestHandler<ProviderCourseLocationsQuery, ProviderCourseLocationsQueryResult>
    {
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;

        public ProviderCourseLocationsQueryHandler(IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
        }

        public async Task<ProviderCourseLocationsQueryResult> Handle(ProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetAllProviderCourseLocations(request.ProviderCourseId);
            var result = new ProviderCourseLocationsQueryResult();
            result.ProviderCourseLocations = providerCourseLocations.Select(x => (ProviderCourseLocationModel)x).ToList();
            return result;
        }
    }
}
