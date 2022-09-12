using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class CreateProviderCourseCommandHandler : IRequestHandler<CreateProviderCourseCommand, int>
    {
        private readonly IProvidersReadRepository _providerReadRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCoursesWriteRepository _providerCourseEditRepository;
        private readonly IRegionsReadRepository _regionReadRepository;
        private readonly ILogger<CreateProviderCourseCommandHandler> _logger;

        public CreateProviderCourseCommandHandler(
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProvidersReadRepository providerReadRepository,
            IProviderCoursesWriteRepository providerCourseEditRepository,
            ILogger<CreateProviderCourseCommandHandler> logger,
            IRegionsReadRepository regionReadRepository)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerReadRepository = providerReadRepository;
            _providerCourseEditRepository = providerCourseEditRepository;
            _logger = logger;
            _regionReadRepository = regionReadRepository;
        }

        public async Task<int> Handle(CreateProviderCourseCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);

            _logger.LogInformation("Adding course: {larscode} to provider: {ukprn} with id:{providerid}", request.LarsCode, request.Ukprn, provider.Id);

            Domain.Entities.ProviderCourse providerCourse = request;
            providerCourse.ProviderId = provider.Id;
            var allProviderLocations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);

            if (request.HasNationalDeliveryOption)
            {
                AddNationaLocationToProviderCourse(request, provider, allProviderLocations, providerCourse);
            }

            //It is assumed here that HasNationalDeliveryOption is false otherwise the validator will throw exception
            if(request.SubregionIds != null && request.SubregionIds.Count > 0)
            {
                await AddRegionalLocationsToProviderCourse(request, provider.Id, allProviderLocations, providerCourse);
            }

            await ProcessProviderLocations(request, providerCourse);

            await _providerCourseEditRepository.CreateProviderCourse(providerCourse);

            _logger.LogInformation("Added course:{larscode} with id:{providercourseid} for provider: {ukprn}", request.LarsCode, providerCourse.Id, request.Ukprn);
            return providerCourse.Id;
        }

        private async Task AddRegionalLocationsToProviderCourse(CreateProviderCourseCommand request, int providerId, List<ProviderLocation> allProviderLocations, Domain.Entities.ProviderCourse providerCourse)
        {
            var allRegions = await _regionReadRepository.GetAllRegions();

            request.SubregionIds.ForEach(regionId => 
            {
                var providerLocation = allProviderLocations.FirstOrDefault(l => l.RegionId == regionId);
                if (providerLocation == null)
                {
                    var region = allRegions.FirstOrDefault(r => r.Id == regionId);
                    providerLocation = ProviderLocation.CreateRegionalLocation(providerId, region);
                    providerCourse.Locations.Add(new ProviderCourseLocation { NavigationId = Guid.NewGuid(), IsImported = false, Location = providerLocation });
                }
                else
                {
                    providerCourse.Locations.Add(new ProviderCourseLocation { NavigationId = Guid.NewGuid(), IsImported = false, ProviderLocationId = providerLocation.Id });
                }
            });
        }

        private void AddNationaLocationToProviderCourse(CreateProviderCourseCommand request, Provider provider, List<ProviderLocation> allProviderLocations, Domain.Entities.ProviderCourse providerCourse)
        {
            var nationalLocation = allProviderLocations.FirstOrDefault(l => l.LocationType == LocationType.National);
            if (nationalLocation == null)
            {
                _logger.LogInformation("Provider:{ukprn} delivers course:{larscode} nationally, there is no national location, creating a new one", request.Ukprn, request.LarsCode);
                nationalLocation = ProviderLocation.CreateNationalLocation(provider.Id);
                providerCourse.Locations.Add(new ProviderCourseLocation { NavigationId = Guid.NewGuid(), Location = nationalLocation });
            }
            else
            {
                _logger.LogInformation("Provider:{ukprn} delivers course:{larscode} nationally, existing national location id:{providerlocationid} will be used", request.Ukprn, request.LarsCode, nationalLocation.Id);
                providerCourse.Locations.Add(new ProviderCourseLocation { NavigationId = Guid.NewGuid(), ProviderLocationId = nationalLocation.Id });
            }
        }


        private async Task ProcessProviderLocations(CreateProviderCourseCommand request, Domain.Entities.ProviderCourse providerCourse)
        {
            if (request.ProviderLocations != null && request.ProviderLocations.Any())
            {
                var allProviderLocations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
                foreach (var providerLocation in request.ProviderLocations)
                {
                    var providerLocationToAdd =
                        allProviderLocations.First(x => x.NavigationId == providerLocation.ProviderLocationId);
                    providerCourse.Locations.Add(new ProviderCourseLocation
                    {
                        NavigationId = Guid.NewGuid(), 
                        ProviderLocationId = providerLocationToAdd.Id,
                        HasBlockReleaseDeliveryOption = providerLocation.HasBlockReleaseDeliveryOption,
                        HasDayReleaseDeliveryOption = providerLocation.HasDayReleaseDeliveryOption
                    });
                }
            }
        }
    }
}
