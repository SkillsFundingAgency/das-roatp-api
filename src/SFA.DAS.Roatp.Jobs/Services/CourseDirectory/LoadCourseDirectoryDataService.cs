using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services.Metrics;
using Standard = SFA.DAS.Roatp.Domain.Entities.Standard;

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

        public async Task<CourseDirectoryImportMetrics> LoadCourseDirectoryData(bool betaAndPilotProvidersOnly)
        {
            var standards = await GetStandards();
            var regions = await GetRegions();

            var cdProviders = await _getCourseDirectoryDataService.GetCourseDirectoryData();

           

            var betaAndPilotProviderMetrics = (BetaAndPilotProviderMetrics)null;
            if (betaAndPilotProvidersOnly)
                betaAndPilotProviderMetrics = await _courseDirectoryDataProcessingService.RemoveProvidersNotOnBetaOrPilotList(cdProviders);
            else
                await _courseDirectoryDataProcessingService.RemoveProvidersNotActiveOnRegister(cdProviders);

            await _courseDirectoryDataProcessingService.RemoveProvidersAlreadyPresentOnRoatp(cdProviders);

            var loadMetrics = new CourseDirectoryImportMetrics
            {
                ProvidersToLoad = cdProviders.Count,
                LocationDuplicationMetrics = new LocationDuplicationMetrics(),
                LarsCodeDuplicationMetrics = new LarsCodeDuplicationMetrics(),
                BetaAndPilotProvidersOnly = betaAndPilotProvidersOnly,
                BetaAndPilotProviderMetrics = betaAndPilotProviderMetrics
            };

            foreach (var cdProvider in cdProviders)
            {
                await CleanseDuplicateLocationNames(cdProvider, loadMetrics.LocationDuplicationMetrics);
                await CleanseDuplicateLarsCodes(cdProvider, loadMetrics.LarsCodeDuplicationMetrics);
                
                var(successMapping, provider) = await _courseDirectoryDataProcessingService.MapCourseDirectoryProvider(cdProvider, standards, regions);

                if (!successMapping)
                {
                    _logger.LogWarning("Ukprn {ukprn} failed to map", cdProvider.Ukprn);
                    loadMetrics.FailedMappings++;
                }
                else
                {
                    await _courseDirectoryDataProcessingService.AugmentPilotData(provider);
                    var loadMetricsProvider = await _importCourseDirectoryDataService.ImportProviders(provider);
                    loadMetrics.SuccessfulLoads += loadMetricsProvider.SuccessfulLoads;
                    loadMetrics.FailedLoads += loadMetricsProvider.FailedLoads;
                }
            }
            
            return loadMetrics;
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

            _logger.LogInformation("Retrieved {count} regions from Region table", regions.Count);
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

            _logger.LogInformation("Retrieved {count} standards from Standard table", standards?.Count);
            return standards;
        }
    }
}
