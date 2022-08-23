using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse
{
    public class CreateProviderCourseCommandHandler : IRequestHandler<CreateProviderCourseCommand, int>
    {
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseEditRepository _providerCourseEditRepository;
        private readonly ILogger<CreateProviderCourseCommandHandler> _logger;

        public CreateProviderCourseCommandHandler(IProviderLocationsReadRepository providerLocationsReadRepository, IProviderReadRepository providerReadRepository, IProviderCourseEditRepository providerCourseEditRepository, ILogger<CreateProviderCourseCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerReadRepository = providerReadRepository;
            _providerCourseEditRepository = providerCourseEditRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateProviderCourseCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);

            _logger.LogInformation("Adding course: {larscode} to provider: {ukprn} with id:{providerid}", request.LarsCode, request.Ukprn, provider.Id);

            Domain.Entities.ProviderCourse providerCourse = request;
            providerCourse.ProviderId = provider.Id;

            if (request.HasNationalDeliveryOption)
            {
                var allProviderLocations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
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

            await ProcessProviderLocations(request, providerCourse);

            await _providerCourseEditRepository.CreateProviderCourse(providerCourse);
            
            _logger.LogInformation("Added course:{larscode} with id:{providercourseid} for provider: {ukprn}", request.LarsCode, providerCourse.Id, request.Ukprn);
            return providerCourse.Id;
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
                        NavigationId = Guid.NewGuid(), ProviderLocationId = providerLocationToAdd.Id,
                        HasBlockReleaseDeliveryOption = providerLocation.HasBlockReleaseDeliveryOption,
                        HasDayReleaseDeliveryOption = providerLocation.HasDayReleaseDeliveryOption
                    });
                }
            }
        }
    }
}
