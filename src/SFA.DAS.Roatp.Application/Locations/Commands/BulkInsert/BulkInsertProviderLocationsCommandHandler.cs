﻿using MediatR;
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
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IRegionReadRepository _regionReadRepository;
        private readonly IProviderLocationsInsertRepository _providerLocationsInsertRepository;
        private readonly ILogger<BulkInsertProviderLocationsCommandHandler> _logger;

        public BulkInsertProviderLocationsCommandHandler(IProviderReadRepository providerReadRepository, IProviderLocationsReadRepository providerLocationsReadRepository,
            IRegionReadRepository regionReadRepository, IProviderLocationsInsertRepository providerLocationsInsertRepository,
             ILogger<BulkInsertProviderLocationsCommandHandler> logger)
        {
            _providerReadRepository = providerReadRepository;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _regionReadRepository = regionReadRepository;
            _providerLocationsInsertRepository = providerLocationsInsertRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkInsertProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(command.Ukprn);
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);

            var regions = await _regionReadRepository.GetAllRegions();
            List<ProviderLocation> locationsToInsert = new List<ProviderLocation>();
            foreach (var i in command.SelectedSubregionIds)
            {
                var region = regions.FirstOrDefault(r => r.Id == i);
                var providerLocation = new ProviderLocation();
                providerLocation.LocationType = LocationType.Regional;
                providerLocation.NavigationId = System.Guid.NewGuid();
                providerLocation.RegionId = region.Id;
                providerLocation.LocationName = region.SubregionName;
                providerLocation.Latitude = region.Latitude;
                providerLocation.Longitude = region.Longitude;
                providerLocation.ProviderId = provider.Id;
                locationsToInsert.Add(providerLocation);
            }
            _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", locationsToInsert.Count(), LocationType.Regional, command.Ukprn);
            await _providerLocationsInsertRepository.BulkInsert(locationsToInsert);
            return locationsToInsert.Count();
        }
    }
}