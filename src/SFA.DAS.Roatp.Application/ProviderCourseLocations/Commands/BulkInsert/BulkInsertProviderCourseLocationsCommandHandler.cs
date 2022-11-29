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

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommandHandler : IRequestHandler<BulkInsertProviderCourseLocationsCommand, int>
    {
        private readonly IProviderCourseLocationsBulkRepository _providerCourseLocationsBulkRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCoursesReadRepository _providerCoursesReadRepository;
        private readonly ILogger<BulkInsertProviderCourseLocationsCommandHandler> _logger;

        public BulkInsertProviderCourseLocationsCommandHandler(IProviderCourseLocationsBulkRepository providerCourseLocationsBulkRepository,
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderCoursesReadRepository providerCoursesReadRepository,
            ILogger<BulkInsertProviderCourseLocationsCommandHandler> logger)
        {
            _providerCourseLocationsBulkRepository = providerCourseLocationsBulkRepository;
            _logger = logger;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerCoursesReadRepository = providerCoursesReadRepository;
        }

        public async Task<int> Handle(BulkInsertProviderCourseLocationsCommand command, CancellationToken cancellationToken)
        {
            var providerCourses = await _providerCoursesReadRepository.GetAllProviderCourses(command.Ukprn);
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);

            List<ProviderCourseLocation> providerCourseLocationsToInsert = new List<ProviderCourseLocation>();
            foreach (var i in command.SelectedSubregionIds)
            {
                var providerCourseLocation = new ProviderCourseLocation
                {
                    NavigationId = System.Guid.NewGuid(),
                    ProviderCourseId = providerCourses.First(a => a.LarsCode == command.LarsCode).Id,
                    ProviderLocationId = providerLocations.First(a => a.RegionId == i).Id
                };
                providerCourseLocationsToInsert.Add(providerCourseLocation);
            }
            _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", providerCourseLocationsToInsert.Count, LocationType.Regional, command.Ukprn);
            await _providerCourseLocationsBulkRepository.BulkInsert(providerCourseLocationsToInsert, command.UserId, command.UserDisplayName, command.Ukprn, AuditEventTypes.BulkInsertProviderCourseLocation);
            return providerCourseLocationsToInsert.Count;
        }
    }
}
