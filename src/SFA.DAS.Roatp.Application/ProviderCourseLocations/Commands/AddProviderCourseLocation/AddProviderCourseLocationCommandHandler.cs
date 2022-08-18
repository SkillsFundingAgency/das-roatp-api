using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommandHandler : IRequestHandler<AddProviderCourseLocationCommand, int>
    {
        private readonly IProviderCourseLocationWriteRepository _providerCourseLocationWriteRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly ILogger<AddProviderCourseLocationCommandHandler> _logger;

        public AddProviderCourseLocationCommandHandler(IProviderCourseLocationWriteRepository providerCourseLocationWriteRepository,
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository,
            ILogger<AddProviderCourseLocationCommandHandler> logger)
        {
            _providerCourseLocationWriteRepository = providerCourseLocationWriteRepository;
            _logger = logger;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerCourseReadRepository = providerCourseReadRepository;
        }

        public async Task<int> Handle(AddProviderCourseLocationCommand command, CancellationToken cancellationToken)
        {
            var providerCourse = await _providerCourseReadRepository.GetProviderCourseByUkprn(command.Ukprn, command.LarsCode);
            var providerLocation = await _providerLocationsReadRepository.GetProviderLocation(command.Ukprn, command.LocationNavigationId);

            var providerCourseLocation = new ProviderCourseLocation
            {
                NavigationId = System.Guid.NewGuid(),
                ProviderCourseId = providerCourse.Id,
                ProviderLocationId = providerLocation.Id,
                HasDayReleaseDeliveryOption = command.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = command.HasBlockReleaseDeliveryOption
            };
            _logger.LogInformation("Creating provider course location for Ukprn: {ukprn} LarsCode: {larsCode}, ProviderLocationId: {Id}", command.Ukprn, command.LarsCode, providerLocation.Id);
            var createdProviderCourseLocation = await _providerCourseLocationWriteRepository.Create(providerCourseLocation);
            return createdProviderCourseLocation.Id;
        }
    }
}
