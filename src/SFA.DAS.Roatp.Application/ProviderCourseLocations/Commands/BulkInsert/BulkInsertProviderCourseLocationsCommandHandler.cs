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
        private readonly IProviderCourseLocationsDeleteRepository _providerCourseLocationsDeleteRepository;
        private readonly IProviderCourseLocationReadRepository _providerCourseLocationReadRepository;
        private readonly IProviderLocationsReadRepository _providerLocationsReadRepository;
        private readonly IProviderCourseReadRepository _providerCourseReadRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<BulkInsertProviderCourseLocationsCommandHandler> _logger;

        public BulkInsertProviderCourseLocationsCommandHandler(IProviderCourseLocationsInsertRepository providerCourseLocationsInsertRepository, IProviderCourseLocationsDeleteRepository providerCourseLocationsDeleteRepository,
            IProviderCourseLocationReadRepository providerCourseLocationReadRepository, ILogger<BulkInsertProviderCourseLocationsCommandHandler> logger, IProviderLocationsReadRepository providerLocationsReadRepository, IProviderCourseReadRepository providerCourseReadRepository, IProviderReadRepository providerReadRepository)
        {
            _providerCourseLocationsInsertRepository = providerCourseLocationsInsertRepository;
            _providerCourseLocationsDeleteRepository = providerCourseLocationsDeleteRepository;
            _providerCourseLocationReadRepository = providerCourseLocationReadRepository;
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
            var providerCourseLocations = await _providerCourseLocationReadRepository.GetAllProviderCourseLocations(command.Ukprn, command.LarsCode);

            List<ProviderCourseLocation> providerCourseLocationsToInsert = new List<ProviderCourseLocation>();
            foreach (var i in command.SelectedSubregionIds)
            {
                var providerCourseLocation = new ProviderCourseLocation();
                providerCourseLocation.NavigationId = System.Guid.NewGuid();
                providerCourseLocation.ProviderCourseId = providerCourses.First(a => a.LarsCode == command.LarsCode).Id;
                providerCourseLocation.ProviderLocationId = providerLocations.First(a => a.RegionId == i).Id;
                providerCourseLocationsToInsert.Add(providerCourseLocation);
            }
            _logger.LogInformation("{count} {locationType} locations will be inserted for Ukprn:{ukprn}", providerCourseLocationsToInsert.Count(), LocationType.Regional, command.Ukprn);
            await _providerCourseLocationsInsertRepository.BulkInsert(providerCourseLocationsToInsert);
            return providerCourseLocationsToInsert.Count();
        }
    }
}
