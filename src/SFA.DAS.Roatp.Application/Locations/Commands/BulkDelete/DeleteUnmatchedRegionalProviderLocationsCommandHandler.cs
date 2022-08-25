using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class DeleteUnmatchedRegionalProviderLocationsCommandHandler : IRequestHandler<DeleteUnmatchedRegionalProviderLocationsCommand, int>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;
        private readonly IProviderLocationsDeleteRepository _providerLocationsDeleteRepository;
        private readonly ILogger<DeleteUnmatchedRegionalProviderLocationsCommandHandler> _logger;

        public DeleteUnmatchedRegionalProviderLocationsCommandHandler(IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderLocationsDeleteRepository providerLocationsDeleteRepository,
            IProviderCourseLocationReadRepository providerCourseLocationReadRepository,
            ILogger<DeleteUnmatchedRegionalProviderLocationsCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerLocationsDeleteRepository = providerLocationsDeleteRepository;
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteUnmatchedRegionalProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetProviderCourseLocationsByUkprn(command.Ukprn);

            var providerLocationIdsToDelete = providerLocations
                .Where(l => l.LocationType == LocationType.Regional)
                .Select(providerLocation => providerLocation.Id)
                .Where(providerLocationId => providerCourseLocations.All(a => a.ProviderLocationId != providerLocationId)).ToList();

            if (providerLocationIdsToDelete.Any())
            {
                _logger.LogInformation("{count} unmatched Regional locations will be deleted for Ukprn:{ukprn}", providerLocationIdsToDelete.Count, command.Ukprn);
                await _providerLocationsDeleteRepository.BulkDelete(providerLocationIdsToDelete);
            }
            return providerLocationIdsToDelete.Count;
        }
    }
}
