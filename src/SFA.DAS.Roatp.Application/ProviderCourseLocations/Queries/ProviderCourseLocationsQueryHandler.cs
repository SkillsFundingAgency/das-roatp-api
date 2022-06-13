using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQueryHandler : IRequestHandler<ProviderCourseLocationsQuery, ProviderCourseLocationsQueryResult>
    {
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;

        public ProviderCourseLocationsQueryHandler(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseLocationReadRepository providerCourseLocationReadRepository)
        {
            _providerReadRepository = providerReadRepository;
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
            _providerCourseReadRepository = providerCourseReadRepository;
        }

        public async Task<ProviderCourseLocationsQueryResult> Handle(ProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);
            var providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, request.LarsCode);
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetAllProviderCourseLocations(providerCourse.Id);
            var result = new ProviderCourseLocationsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations.Select(x => (ProviderCourseLocationModel)x).ToList()
            };

            return result;
        }
    }
}
