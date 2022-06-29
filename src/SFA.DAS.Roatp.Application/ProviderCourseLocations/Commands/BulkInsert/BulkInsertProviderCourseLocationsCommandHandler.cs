using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkInsert
{
    public class BulkInsertProviderCourseLocationsCommandHandler : IRequestHandler<BulkInsertProviderCourseLocationsCommand, int>
    {
        private readonly IProviderCourseLocationsInsertRepository _providerCourseLocationsInsertRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<BulkInsertProviderCourseLocationsCommandHandler> _logger;

        public BulkInsertProviderCourseLocationsCommandHandler(IProviderCourseLocationsInsertRepository providerCourseLocationsInsertRepository,
            IProviderLocationsReadRepository providerLocationsReadRepository,
            IProviderCourseReadRepository providerCourseReadRepository,
            IProviderReadRepository providerReadRepository,
            ILogger<BulkInsertProviderCourseLocationsCommandHandler> logger)
        {
            _providerCourseLocationsInsertRepository = providerCourseLocationsInsertRepository;
            _logger = logger;
            _providerLocationsReadRepository = providerLocationsReadRepository;
            _providerCourseReadRepository = providerCourseReadRepository;
            _providerReadRepository = providerReadRepository;
        }

        public async Task<int> Handle(BulkInsertProviderCourseLocationsCommand command, CancellationToken cancellationToken)
        {
            var provider = await _providerReadRepository.GetByUkprn(command.Ukprn);
            var providerLocations = await _providerLocationsReadRepository.GetAllProviderLocations(command.Ukprn);
            var providerCourses = await _providerCourseReadRepository.GetAllProviderCourses(provider.Id);

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
            _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", providerCourseLocationsToInsert.Count(), LocationType.Regional, command.Ukprn);
            await _providerCourseLocationsInsertRepository.BulkInsert(providerCourseLocationsToInsert);
            return providerCourseLocationsToInsert.Count();
        }
    }
}
