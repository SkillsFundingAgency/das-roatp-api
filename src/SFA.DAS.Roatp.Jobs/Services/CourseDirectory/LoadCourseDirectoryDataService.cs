using System.Threading.Tasks;
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

            var loadMetrics = await _importCourseDirectoryDataService.ImportCourseDirectoryData(cdProviders);
            
            if (loadMetrics.FailedLoads==0 && loadMetrics.FailedMappings==0)
                _logger.LogInformation($"Load providers from course directory successful with no issues: {loadMetrics.ProvidersToLoad} for loading, {loadMetrics.SuccessfulLoads} loaded");
            else
            {
                _logger.LogInformation($"Load providers from course directory did not fully load successfully: {loadMetrics.SuccessfulLoads} successfully loaded, {loadMetrics.FailedMappings} failed to map, {loadMetrics.FailedLoads} failed to load");
            }
        }
    }
}
