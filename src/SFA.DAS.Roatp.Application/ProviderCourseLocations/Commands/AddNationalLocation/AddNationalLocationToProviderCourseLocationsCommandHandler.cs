using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.Roatp.Domain.Constants;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation
{
    public class AddNationalLocationToProviderCourseLocationsCommandHandler : IRequestHandler<AddNationalLocationToProviderCourseLocationsCommand, ProviderCourseLocation>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderLocationWriteRepository _providerLocationWriteRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderCourseLocationWriteRepository _providerCourseLocationWriteRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<AddNationalLocationToProviderCourseLocationsCommandHandler> _logger;

        public AddNationalLocationToProviderCourseLocationsCommandHandler(
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderLocationWriteRepository providerLocationWriteRepository,
            IProviderCourseReadRepository providerCourseReadRepository,
            IProviderCourseLocationWriteRepository providerCourseLocationWriteRepository,
            IProviderReadRepository providerReadRepository,
            ILogger<AddNationalLocationToProviderCourseLocationsCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerLocationWriteRepository = providerLocationWriteRepository;
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerCourseLocationWriteRepository = providerCourseLocationWriteRepository;
            _providerReadRepository = providerReadRepository;
            _logger = logger;
        }

        public async Task<ProviderCourseLocation> Handle(AddNationalLocationToProviderCourseLocationsCommand request, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(request.Ukprn);
            var allLocations = await _providerLocationsReadRepository.GetAllProviderLocations(request.Ukprn);
            var nationalLocation = allLocations.SingleOrDefault(l => l.LocationType == LocationType.National);
            if (nationalLocation == null)
            {
                _logger.LogInformation("Creating national location for Ukprn: {ukprn}", request.Ukprn);
                nationalLocation = await _providerLocationWriteRepository.Create(new ProviderLocation
                {
                    NavigationId = Guid.NewGuid(),
                    ProviderId = provider.Id,
                    Latitude = NationalLatLong.NationalLatitude,
                    Longitude = NationalLatLong.NationalLongitude,
                    LocationType = LocationType.National
                });
            }

            var providerCourse = await _providerCourseReadRepository.GetProviderCourse(provider.Id, request.LarsCode);

            var providerCourseLocation = new ProviderCourseLocation
            {
                NavigationId = Guid.NewGuid(),
                ProviderLocationId = nationalLocation.Id,
                ProviderCourseId = providerCourse.Id
            };

            _logger.LogInformation($"Associating national location for ProviderCourse:{providerCourse.Id}");

            return await _providerCourseLocationWriteRepository.Create(providerCourseLocation);
        }
    }
}
