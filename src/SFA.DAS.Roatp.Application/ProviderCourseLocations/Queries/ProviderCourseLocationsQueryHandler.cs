using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries
{
    public class ProviderCourseLocationsQueryHandler : IRequestHandler<ProviderCourseLocationsQuery, ProviderCourseLocationsQueryResult>
    {
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;
        private readonly ILogger<ProviderCourseLocationsQueryHandler> _logger;

        public ProviderCourseLocationsQueryHandler(IProviderReadRepository providerReadRepository, IProviderCourseReadRepository providerCourseReadRepository, IProviderCourseLocationReadRepository providerCourseLocationReadRepository, ILogger<ProviderCourseLocationsQueryHandler> logger)
        {
            _providerReadRepository = providerReadRepository;
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
            _providerCourseReadRepository = providerCourseReadRepository;
            _logger = logger;
        }

        public async Task<ProviderCourseLocationsQueryResult> Handle(ProviderCourseLocationsQuery request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);
            if (provider == null)
            {
                _logger.LogInformation("Provider data not found for {ukprn} and {larsCode}", request.Ukprn, request.LarsCode);
                return null;
            }

            var providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, request.LarsCode);
            if (providerCourse == null)
            {
                _logger.LogInformation("Provider Course data not found for {ukprn} and {larsCode}", request.Ukprn, request.LarsCode);
                return null;
            }
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetAllProviderCourseLocations(providerCourse.Id);
            if (!providerCourseLocations.Any())
            {
                _logger.LogInformation("Provider Course Locations data not found for {ukprn} and {larsCode}", request.Ukprn, request.LarsCode);
                return null;
            }
            var result = new ProviderCourseLocationsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations.Select(x => (ProviderCourseLocationModel)x).ToList()
            };

            return result;
        }
    }
}
