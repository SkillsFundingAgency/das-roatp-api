using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommandHandler : IRequestHandler<BulkDeleteProviderLocationsCommand, int>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;
        private readonly IProviderLocationsDeleteRepository _providerLocationsDeleteRepository;
        private readonly ILogger<BulkDeleteProviderLocationsCommandHandler> _logger;

        public BulkDeleteProviderLocationsCommandHandler(IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderLocationsDeleteRepository providerLocationsDeleteRepository,
            IProviderCourseLocationReadRepository providerCourseLocationReadRepository,
            ILogger<BulkDeleteProviderLocationsCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerLocationsDeleteRepository = providerLocationsDeleteRepository;
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkDeleteProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetProviderCourseLocationsByUkprn(command.Ukprn);

            var providerLocationIdsToDelete = new List<int>();
            foreach (var providerLocationId in providerLocations.Select(providerLocation => providerLocation.Id))
            {
                var hasProviderCourseLocations = providerCourseLocations.Any(a => a.ProviderLocationId == providerLocationId);
                if (!hasProviderCourseLocations)
                {
                    providerLocationIdsToDelete.Add(providerLocationId);
                }
            }
            if(providerLocationIdsToDelete.Any())
            {
                _logger.LogInformation("{count} {locationType} locations will be deleted for Ukprn:{ukprn}", providerLocationIdsToDelete.Count, LocationType.Regional, command.Ukprn);
                await _providerLocationsDeleteRepository.BulkDelete(providerLocationIdsToDelete);
            }
            return providerLocationIdsToDelete.Count;
        }
    }
}
