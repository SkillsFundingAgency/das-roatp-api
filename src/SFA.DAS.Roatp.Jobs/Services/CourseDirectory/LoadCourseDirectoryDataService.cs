using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class LoadCourseDirectoryDataService: ILoadCourseDirectoryDataService
    {
        private readonly IGetCourseDirectoryDataService _getCourseDirectoryDataService;
        private readonly ICourseDirectoryDataProcessingService _courseDirectoryDataProcessingService;
        private readonly IImportCourseDirectoryDataService _importCourseDirectoryDataService;
        private readonly IStandardReadRepository _standardReadReadRepository;
        private readonly IRegionReadRepository _regionReadRepository;
        private readonly ILogger<LoadCourseDirectoryDataService> _logger;

        public LoadCourseDirectoryDataService(IGetCourseDirectoryDataService getCourseDirectoryDataService,   ICourseDirectoryDataProcessingService courseDirectoryDataProcessingService, IImportCourseDirectoryDataService importCourseDirectoryDataService,  IStandardReadRepository standardReadReadRepository, IRegionReadRepository regionReadRepository, ILogger<LoadCourseDirectoryDataService> logger)
        {
            _getCourseDirectoryDataService = getCourseDirectoryDataService;
            _courseDirectoryDataProcessingService = courseDirectoryDataProcessingService;
            _importCourseDirectoryDataService = importCourseDirectoryDataService;
            _standardReadReadRepository = standardReadReadRepository;
            _regionReadRepository = regionReadRepository;
            _logger = logger;
        }

        public async Task LoadCourseDirectoryData()
        {
            var standards = await GetStandards();
            var regions = await GetRegions();

            var cdProviders = await _getCourseDirectoryDataService.GetCourseDirectoryData();
            
            await _courseDirectoryDataProcessingService.RemoveProvidersNotActiveOnRegister(cdProviders);
            await _courseDirectoryDataProcessingService.RemoveProvidersAlreadyPresentOnRoatp(cdProviders);

            var betaProvidersOnly = true;  // does this need a switch/feature flag?

            // this will need to handle beta providers
            // put in flag in http trigger to be 'beta only', all etc
            if (betaProvidersOnly)
                await _courseDirectoryDataProcessingService.RemoveProvidersNotOnBetaList(cdProviders);

            var locationDuplicationMetrics = new LocationDuplicationMetrics();
            var larsCodeDuplicationMetrics = new LarsCodeDuplicationMetrics();

            var loadMetrics = new CourseDirectoryImportMetrics
            {
                ProvidersToLoad = cdProviders.Count
            };

            foreach (var cdProvider in cdProviders)
            {
                await CleanseDuplicateLocationNames(cdProvider, locationDuplicationMetrics);
                await CleanseDuplicateLarsCodes(cdProvider, larsCodeDuplicationMetrics);
                await _courseDirectoryDataProcessingService.InsertMissingPilotData(cdProvider);
                
                var(successMapping, provider) = await _courseDirectoryDataProcessingService.MapCourseDirectoryProvider(cdProvider, standards, regions);

                if (!successMapping)
                {
                    _logger.LogWarning($"Ukprn {cdProvider.Ukprn} failed to map");
                    loadMetrics.FailedMappings++;
                }
                else
                {
                    var loadMetricsProvider =
                        await _importCourseDirectoryDataService.ImportProviders(provider);
                    loadMetrics.SuccessfulLoads += loadMetricsProvider.SuccessfulLoads;
                    loadMetrics.FailedLoads += loadMetricsProvider.FailedLoads;
                }
            }
            
            LogMetrics(loadMetrics, locationDuplicationMetrics, larsCodeDuplicationMetrics, betaProvidersOnly);
        }

        private void LogMetrics(CourseDirectoryImportMetrics loadMetrics, LocationDuplicationMetrics locationDuplicationMetrics,
            LarsCodeDuplicationMetrics larsCodeDuplicationMetrics, bool betaProvidersOnly)
        {
            if (loadMetrics.FailedLoads == 0 && loadMetrics.FailedMappings == 0 &&
                locationDuplicationMetrics.ProviderLocationsRemoved == 0 &&
                larsCodeDuplicationMetrics.ProviderStandardsRemoved == 0)
                _logger.LogInformation(
                    $"Load providers from course directory beta providers only: [{betaProvidersOnly}] successful with no issues: {loadMetrics.ProvidersToLoad} for loading, {loadMetrics.SuccessfulLoads} loaded");
            else
            {
                _logger.LogInformation($"Load providers from course directory did not fully load successfully: " +
                                       $"{loadMetrics.SuccessfulLoads} successfully loaded ( beta providers only: [{betaProvidersOnly}] ), {loadMetrics.FailedMappings} failed to map, " +
                                       $"{loadMetrics.FailedLoads} failed to load, " +
                                       $"{locationDuplicationMetrics.ProvidersWithDuplicateLocationNames} providers with duplicate locations removed" +
                                       $"{locationDuplicationMetrics.ProviderLocationsRemoved} total duplicate locations removed" +
                                       $"{larsCodeDuplicationMetrics.ProviderStandardsRemoved} providers with duplicate standards removed" +
                                       $"{larsCodeDuplicationMetrics.ProvidersWithDuplicateStandards} total duplicate standards removed");
            }
        }

        private async Task CleanseDuplicateLarsCodes(CdProvider cdProvider,
            LarsCodeDuplicationMetrics larsCodeDuplicationMetrics)
        {
            var metrics = await _courseDirectoryDataProcessingService.CleanseDuplicateLarsCodes(cdProvider);
            larsCodeDuplicationMetrics.ProviderStandardsRemoved += metrics.ProviderStandardsRemoved;
            larsCodeDuplicationMetrics.ProvidersWithDuplicateStandards += metrics.ProvidersWithDuplicateStandards;
        }

        private async Task CleanseDuplicateLocationNames(CdProvider cdProvider,
            LocationDuplicationMetrics locationDuplicationMetrics)
        {
            var metrics = await _courseDirectoryDataProcessingService.CleanseDuplicateLocationNames(cdProvider);
            locationDuplicationMetrics.ProviderLocationsRemoved += metrics.ProviderLocationsRemoved;
            locationDuplicationMetrics.ProvidersWithDuplicateLocationNames += metrics.ProvidersWithDuplicateLocationNames;
        }

        private async Task<List<Region>> GetRegions()
        {
            var regions = await _regionReadRepository.GetAllRegions();
            if (regions == null || regions.Count == 0)
            {
                var errorMessage = "No regions could be retrieved from the regions table";
                _logger.LogError(errorMessage);
                throw new InvalidDataException(errorMessage);
            }

            _logger.LogInformation($"Retrieved {regions.Count} regions from Region table");
            return regions;
        }

        private async Task<List<Standard>> GetStandards()
        {
            var standards = await _standardReadReadRepository.GetAllStandards();
            if (standards == null || standards.Count == 0)
            {
                var errorMessage = "No standards could be retrieved from the standards cache";
                _logger.LogError(errorMessage);
                throw new InvalidDataException(errorMessage);
            }

            _logger.LogInformation($"Retrieved {standards?.Count} standards from Standard table");
            return standards;
        }
    }
}
