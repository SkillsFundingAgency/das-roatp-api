using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommandHandler : IRequestHandler<BulkDeleteProviderCourseLocationsCommand, int>
    {
        private readonly IProviderCourseLocationsBulkRepository _providerCourseLocationsBulkRepository;
        private readonly IProviderCourseLocationsReadRepository _providerCourseLocationsReadRepository;
        private readonly ILogger<BulkDeleteProviderCourseLocationsCommandHandler> _logger;

        public BulkDeleteProviderCourseLocationsCommandHandler(IProviderCourseLocationsBulkRepository providerCourseLocationsBulkRepository, IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository, ILogger<BulkDeleteProviderCourseLocationsCommandHandler> logger)
        {
            _providerCourseLocationsBulkRepository = providerCourseLocationsBulkRepository;
            _providerCourseLocationsReadRepository = providerCourseLocationsReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkDeleteProviderCourseLocationsCommand request, CancellationToken cancellationToken)
        {
            var courseLocations = await _providerCourseLocationsReadRepository.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode);

            if (!courseLocations.Any())
            {
                _logger.LogInformation("No locations are associated with Ukprn:{ukprn} and LarsCode:{larscode}", request.Ukprn, request.LarsCode);
                return 0;
            }

            IEnumerable<ProviderCourseLocation> locationsToDelete;

            if (request.DeleteProviderCourseLocationOptions == DeleteProviderCourseLocationOption.DeleteProviderLocations)
                locationsToDelete = courseLocations.Where(l => l.Location.LocationType == LocationType.Provider);
            else
                locationsToDelete = courseLocations.Where(l => l.Location.LocationType != LocationType.Provider);

            var count = locationsToDelete.Count();

            var locationType = request.DeleteProviderCourseLocationOptions == DeleteProviderCourseLocationOption.DeleteProviderLocations ? "Provider" : "Employer";
            if (count == 0)
            {
                _logger.LogInformation("No {locationType} locations found for Ukprn:{ukprn} and LarsCode:{larscode}", locationType, request.Ukprn, request.LarsCode);
                return 0;
            }

            _logger.LogInformation("{count} {locationType} locations will be deleted for Ukprn:{ukprn} and LarsCode:{larscode}", count, locationType, request.Ukprn, request.LarsCode);
            await _providerCourseLocationsBulkRepository.BulkDelete(locationsToDelete.Select(l => l.Id));

            return count;
        }
    }
}
