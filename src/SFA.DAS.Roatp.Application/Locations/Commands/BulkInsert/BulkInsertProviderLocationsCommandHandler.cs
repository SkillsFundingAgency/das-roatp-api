using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandHandler : IRequestHandler<BulkInsertProviderLocationsCommand, int>
    {
        private readonly IProviderLocationsInsertRepository _providerLocationsInsertRepository;
        private readonly IProviderLocationsDeleteRepository _providerLocationsDeleteRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IRegionReadRepository _regionReadRepository;
        private readonly ILogger<BulkInsertProviderLocationsCommandHandler> _logger;

        public BulkInsertProviderLocationsCommandHandler(IProviderLocationsInsertRepository providerLocationsInsertRepository, IProviderLocationsDeleteRepository providerLocationsDeleteRepository, 
            IProviderLocationsReadRepository providerLocationsReadRepository, IRegionReadRepository regionReadRepository, ILogger<BulkInsertProviderLocationsCommandHandler> logger)
        {
            _providerLocationsInsertRepository = providerLocationsInsertRepository;
            _providerLocationsDeleteRepository = providerLocationsDeleteRepository;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _regionReadRepository = regionReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkInsertProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);

            if (!providerLocations.Any())
            {
                _logger.LogInformation("No locations are associated with Ukprn:{ukprn}", command.Ukprn);
                return 0;
            }

            //IEnumerable<ProviderLocation> locationsToDelete = providerLocations.Where(l => l.LocationType == LocationType.Regional);

            //var count = locationsToDelete.Count();

            //_logger.LogInformation("{count} {locationType} locations will be deleted for Ukprn:{ukprn}", count, LocationType.Regional, command.Ukprn);
            //await _providerLocationsDeleteRepository.BulkDelete(locationsToDelete.Select(l => l.Id));

            var regions = await _regionReadRepository.GetAllRegions();
            List<ProviderLocation> locationsToInsert = new List<ProviderLocation>();
            foreach (var i in command.SubregionIds)
            {
                var region = regions.FirstOrDefault(r => r.Id == i);
                var providerLocation = new ProviderLocation();
                providerLocation.LocationType = LocationType.Regional;
                providerLocation.NavigationId = System.Guid.NewGuid();
                providerLocation.RegionId = region.Id;
                providerLocation.LocationName = region.SubregionName;
                providerLocation.AddressLine1 = region.SubregionName;
                providerLocation.Latitude = region.Latitude;
                providerLocation.Longitude = region.Longitude;
                providerLocation.ProviderId = providerLocations.FirstOrDefault().ProviderId;
                providerLocation.Email = providerLocations.FirstOrDefault().Email;
                providerLocation.Website = providerLocations.FirstOrDefault().Website;
                providerLocation.Phone = providerLocations.FirstOrDefault().Phone;
                locationsToInsert.Add(providerLocation);
            }
            _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", locationsToInsert.Count(), LocationType.Regional, command.Ukprn);
            await _providerLocationsInsertRepository.BulkInsert(locationsToInsert);
            return locationsToInsert.Count();
        }
    }
}
