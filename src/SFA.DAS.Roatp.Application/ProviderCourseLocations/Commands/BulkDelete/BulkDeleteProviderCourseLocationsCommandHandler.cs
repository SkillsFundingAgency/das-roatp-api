using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete
{
    public class BulkDeleteProviderCourseLocationsCommandHandler : IRequestHandler<BulkDeleteProviderCourseLocationsCommand, ValidatedResponse<int>>
    {
        private readonly IProviderCourseLocationsBulkRepository _providerCourseLocationsBulkRepository;
        private readonly IProviderCourseLocationsReadRepository _providerCourseLocationsReadRepository;
        private readonly ILogger<BulkDeleteProviderCourseLocationsCommandHandler> _logger;

        public BulkDeleteProviderCourseLocationsCommandHandler(IProviderCourseLocationsBulkRepository providerCourseLocationsBulkRepository, IProviderCourseLocationsReadRepository providerCourseLocationsReadRepository, ILogger<BulkDeleteProviderCourseLocationsCommandHandler> logger)
        {
            _providerCourseLocationsBulkRepository = providerCourseLocationsBulkRepository;
            _providerCourseLocationsReadRepository = providerCourseLocationsReadRepository;
            _logger = logger;
        }

        public async Task<ValidatedResponse<int>> Handle(BulkDeleteProviderCourseLocationsCommand command, CancellationToken cancellationToken)
        {
            var courseLocations = await _providerCourseLocationsReadRepository.GetAllProviderCourseLocations(command.Ukprn, command.LarsCode);

            if (!courseLocations.Any())
            {
                _logger.LogInformation("No locations are associated with Ukprn:{ukprn} and LarsCode:{larscode}", command.Ukprn, command.LarsCode);
                return new ValidatedResponse<int>(0);
            }

            IEnumerable<ProviderCourseLocation> locationsToDelete;

            if (command.DeleteProviderCourseLocationOptions == DeleteProviderCourseLocationOption.DeleteProviderLocations)
                locationsToDelete = courseLocations.Where(l => l.Location.LocationType == LocationType.Provider);
            else
                locationsToDelete = courseLocations.Where(l => l.Location.LocationType != LocationType.Provider);

            var count = locationsToDelete.Count();

            var locationType = command.DeleteProviderCourseLocationOptions == DeleteProviderCourseLocationOption.DeleteProviderLocations ? "Provider" : "Employer";
            if (count == 0)
            {
                _logger.LogInformation("No {locationType} locations found for Ukprn:{ukprn} and LarsCode:{larscode}", locationType, command.Ukprn, command.LarsCode);
                return new ValidatedResponse<int>(0);
            }

            _logger.LogInformation("{count} {locationType} locations will be deleted for Ukprn:{ukprn} and LarsCode:{larscode}", count, locationType, command.Ukprn, command.LarsCode);
            await _providerCourseLocationsBulkRepository.BulkDelete(locationsToDelete.Select(l => l.Id), command.UserId, command.UserDisplayName, command.Ukprn, AuditEventTypes.BulkDeleteProviderCourseLocation);

            return new ValidatedResponse<int>(count);
        }
    }
}
