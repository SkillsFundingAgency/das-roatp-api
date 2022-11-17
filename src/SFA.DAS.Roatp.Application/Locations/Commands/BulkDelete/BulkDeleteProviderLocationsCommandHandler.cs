using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete
{
    public class BulkDeleteProviderLocationsCommandHandler : IRequestHandler<BulkDeleteProviderLocationsCommand, int>
    {
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseLocationsReadRepository _providerCourseLocationsReadRepository;
        private readonly IProviderLocationsBulkRepository _providerLocationsBulkRepository;
        private readonly ILogger<BulkDeleteProviderLocationsCommandHandler> _logger;

        public BulkDeleteProviderLocationsCommandHandler(IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderLocationsBulkRepository providerLocationsBulkRepository,
            IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository,
            ILogger<BulkDeleteProviderLocationsCommandHandler> logger)
        {
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerLocationsBulkRepository = providerLocationsBulkRepository;
            _providerCourseLocationsReadRepository = providerCourseLocationsReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(BulkDeleteProviderLocationsCommand command, CancellationToken cancellationToken)
        {
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
            var providerCourseLocations = await _providerCourseLocationsReadRepository.GetProviderCourseLocationsByUkprn(command.Ukprn);

            var providerLocationIdsToDelete = providerLocations
                .Where(l => l.LocationType == LocationType.Regional)
                .Select(providerLocation => providerLocation.Id)
                .Where(providerLocationId => providerCourseLocations.All(a => a.ProviderLocationId != providerLocationId)).ToList();

            if (providerLocationIdsToDelete.Any())
            {
                _logger.LogInformation("{count} {locationType} locations will be deleted for Ukprn:{ukprn}", providerLocationIdsToDelete.Count, LocationType.Regional, command.Ukprn);
                await _providerLocationsBulkRepository.BulkDelete(providerLocationIdsToDelete, command.UserId,  command.UserDisplayName, command.Ukprn, AuditEventTypes.BulkDeleteProviderLocation.ToString());
            }
            return providerLocationIdsToDelete.Count;
        }
    }
}
