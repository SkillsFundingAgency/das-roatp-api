using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommandHandler : IRequestHandler<AddProviderCourseLocationCommand, int>
    {
        private readonly IProviderCourseLocationsWriteRepository _providerCourseLocationsWriteRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly ILogger<AddProviderCourseLocationCommandHandler> _logger;

        public AddProviderCourseLocationCommandHandler(IProviderCourseLocationsWriteRepository providerCourseLocationWriteRepository,
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderCoursesReadRepository providerCoursesReadRepository,
            ILogger<AddProviderCourseLocationCommandHandler> logger)
        {
            _providerCourseLocationsWriteRepository = providerCourseLocationWriteRepository;
            _logger = logger;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerCoursesReadRepository = providerCoursesReadRepository;
        }

        public async Task<int> Handle(AddProviderCourseLocationCommand command, CancellationToken cancellationToken)
        {
            var providerCourse = await _providerCoursesReadRepository.GetProviderCourseByUkprn(command.Ukprn, command.LarsCode);
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
            var createdProviderCourseLocation = await _providerCourseLocationsWriteRepository.Create(providerCourseLocation, command.Ukprn, command.UserId, command.UserDisplayName, AuditEventTypes.CreateProviderCourseLocation);
            return createdProviderCourseLocation.Id;
        }
    }
}
