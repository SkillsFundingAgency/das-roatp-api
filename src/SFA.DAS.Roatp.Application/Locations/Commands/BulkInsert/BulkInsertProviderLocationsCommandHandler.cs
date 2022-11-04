using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert
{
    public class BulkInsertProviderLocationsCommandHandler : IRequestHandler<BulkInsertProviderLocationsCommand, int>
    {
        private readonly IRegionsReadRepository _regionsReadRepository;
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly IProviderLocationsBulkRepository _providerLocationsBulkRepository;
        private readonly ILogger<BulkInsertProviderLocationsCommandHandler> _logger;

        public BulkInsertProviderLocationsCommandHandler(IRegionsReadRepository regionsReadRepository, IProvidersReadRepository providersReadRepository,
            IProviderLocationsBulkRepository providerLocationsBulkRepository,
            ILogger<BulkInsertProviderLocationsCommandHandler> logger)
        {
            _regionsReadRepository = regionsReadRepository;
            _providersReadRepository = providersReadRepository;
            _providerLocationsBulkRepository = providerLocationsBulkRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkInsertProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var provider = await _providersReadRepository.GetByUkprn(command.Ukprn);
            var regions = await _regionsReadRepository.GetAllRegions();

            List<ProviderLocation> locationsToInsert = new List<ProviderLocation>();
            foreach (var selectedSubregionId in command.SelectedSubregionIds)
            {
                var region = regions.FirstOrDefault(r => r.Id == selectedSubregionId);
                var providerLocation = new ProviderLocation
                {
                    LocationType = LocationType.Regional,
                    NavigationId = System.Guid.NewGuid(),
                    RegionId = region.Id,
                    LocationName = region.SubregionName,
                    Latitude = region.Latitude,
                    Longitude = region.Longitude,
                    ProviderId = provider.Id
                };
                locationsToInsert.Add(providerLocation);
            }
            if(locationsToInsert.Any())
            {
                _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", locationsToInsert.Count, LocationType.Regional, command.Ukprn);
                await _providerLocationsBulkRepository.BulkInsert(locationsToInsert);
            }
            return locationsToInsert.Count;
        }
    }
}
