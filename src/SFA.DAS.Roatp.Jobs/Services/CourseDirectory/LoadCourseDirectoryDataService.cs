using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class LoadCourseDirectoryDataService: ILoadCourseDirectoryDataService
    {
        private readonly IGetCourseDirectoryDataService _getCourseDirectoryDataService;
        private readonly ICourseDirectoryDataProcessingService _courseDirectoryDataProcessingService;
        private readonly IImportCourseDirectoryDataService _importCourseDirectoryDataService;

        private readonly ILogger<LoadCourseDirectoryDataService> _logger;

        public LoadCourseDirectoryDataService(IGetCourseDirectoryDataService getCourseDirectoryDataService,   ICourseDirectoryDataProcessingService courseDirectoryDataProcessingService, IImportCourseDirectoryDataService importCourseDirectoryDataService, ILogger<LoadCourseDirectoryDataService> logger)
        {
            _getCourseDirectoryDataService = getCourseDirectoryDataService;
            _courseDirectoryDataProcessingService = courseDirectoryDataProcessingService;
            _importCourseDirectoryDataService = importCourseDirectoryDataService;
            _logger = logger;
        }

        public async Task LoadCourseDirectoryData()
        {
            var cdProviders = await _getCourseDirectoryDataService.GetCourseDirectoryData();
            
            await _courseDirectoryDataProcessingService.RemoveProvidersNotActiveOnRegister(cdProviders);
            await _courseDirectoryDataProcessingService.RemoveProvidersAlreadyPresentOnRoatp(cdProviders);
            var pilotProvidersOnly = true;  // does this need a switch/feature flag?
            if (pilotProvidersOnly)
                await _courseDirectoryDataProcessingService.RemoveProvidersNotOnPilotList(cdProviders);

            var locationDuplicationMetrics = await _courseDirectoryDataProcessingService.CleanseDuplicateLocationNames(cdProviders);
            var larsCodeDuplicationMetrics = await _courseDirectoryDataProcessingService.CleanseDuplicateLarsCodes(cdProviders);

            await _courseDirectoryDataProcessingService.InsertMissingPilotData(cdProviders);

            var loadMetrics = await _importCourseDirectoryDataService.ImportCourseDirectoryData(cdProviders);

            loadMetrics.LocationDuplicationMetrics = locationDuplicationMetrics;
            loadMetrics.LarsCodeDuplicationMetrics = larsCodeDuplicationMetrics;

            if (loadMetrics.FailedLoads==0 && loadMetrics.FailedMappings==0 && locationDuplicationMetrics.ProviderLocationsRemoved==0 && larsCodeDuplicationMetrics.ProviderStandardsRemoved==0)
                _logger.LogInformation($"Load providers from course directory successful with no issues: {loadMetrics.ProvidersToLoad} for loading, {loadMetrics.SuccessfulLoads} loaded");
            else
            {
                _logger.LogInformation($"Load providers from course directory did not fully load successfully: " +
                                       $"{loadMetrics.SuccessfulLoads} successfully loaded, {loadMetrics.FailedMappings} failed to map, " +
                                       $"{loadMetrics.FailedLoads} failed to load, " +
                                       $"{locationDuplicationMetrics.ProvidersWithDuplicateNames} providers with duplicate locations removed" +
                                       $"{locationDuplicationMetrics.ProviderLocationsRemoved} total duplicate locations removed" +
                                       $"{larsCodeDuplicationMetrics.ProviderStandardsRemoved} providers with duplicate standards removed" +
                                       $"{larsCodeDuplicationMetrics.ProvidersWithDuplicateStandards} total duplicate standards removed");
            }
        }
    }
}
