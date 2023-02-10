using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;
using Standard = SFA.DAS.Roatp.Domain.Entities.Standard;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    [ExcludeFromCodeCoverage]
    public class LoadCourseDirectoryDataService: ILoadCourseDirectoryDataService
    {
        private readonly IGetCourseDirectoryDataService _getCourseDirectoryDataService;
        private readonly ICourseDirectoryDataProcessingService _courseDirectoryDataProcessingService;
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly IRegionsReadRepository _regionsReadRepository;
        private readonly ILoadProviderRepository _loadProvider;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;

        private readonly ILogger<LoadCourseDirectoryDataService> _logger;

        public LoadCourseDirectoryDataService(IGetCourseDirectoryDataService getCourseDirectoryDataService,   ICourseDirectoryDataProcessingService courseDirectoryDataProcessingService,   IStandardsReadRepository standardsReadRepository, IRegionsReadRepository regionsReadRepository, ILogger<LoadCourseDirectoryDataService> logger, ILoadProviderRepository loadProvider, IImportAuditWriteRepository importAuditWriteRepository)
        {
            _getCourseDirectoryDataService = getCourseDirectoryDataService;
            _courseDirectoryDataProcessingService = courseDirectoryDataProcessingService;
            _standardsReadRepository = standardsReadRepository;
            _regionsReadRepository = regionsReadRepository;
            _logger = logger;
            _loadProvider = loadProvider;
            _importAuditWriteRepository = importAuditWriteRepository;
        }

        public async Task<CourseDirectoryImportMetrics> LoadCourseDirectoryData(bool pilotProvidersOnly)
        {
            var loadMetrics = new CourseDirectoryImportMetrics()
            {
                LocationDuplicationMetrics = new LocationDuplicationMetrics(),
                LarsCodeDuplicationMetrics = new LarsCodeDuplicationMetrics(),
                PilotProvidersOnly = pilotProvidersOnly
            };

            var localRun = false;
            var env = Environment.GetEnvironmentVariable("EnvironmentName");
            if (string.IsNullOrEmpty(env) || env.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                localRun = true;
            }

            var timeStarted = DateTime.UtcNow;

            var standards = await GetStandards();
            loadMetrics.TotalStandardsInCache = standards.Count;

            var regions =  await GetRegions();

            var cdProviders = await _getCourseDirectoryDataService.GetCourseDirectoryData();
            loadMetrics.TotalProvidersFromCourseDirectory = cdProviders.Count;

            if (pilotProvidersOnly)
            {
                loadMetrics.PilotProviderMetrics = await _courseDirectoryDataProcessingService.RemoveProvidersNotOnPilotList(cdProviders);
            }
            else
            {
                loadMetrics.TotalProvidersOnTheRegister = await _courseDirectoryDataProcessingService.RemoveProvidersNotActiveOnRegister(cdProviders);
            }

            loadMetrics.NumberOfProvidersAlreadyLoaded = await _courseDirectoryDataProcessingService.RemovePreviouslyLoadedProviders(cdProviders);

            loadMetrics.TotalNumberOfProvidersToBeLoaded = cdProviders.Count;

            foreach (var cdProvider in cdProviders)
            {
                await CleanseDuplicateLocationNames(cdProvider, loadMetrics.LocationDuplicationMetrics);
                await CleanseDuplicateLarsCodes(cdProvider, loadMetrics.LarsCodeDuplicationMetrics, localRun);
                
                var(successMapping, provider) = await _courseDirectoryDataProcessingService.MapCourseDirectoryProvider(cdProvider, standards, regions);

                if (!successMapping)
                {
                    _logger.LogWarning("Ukprn {ukprn} failed to map", cdProvider.Ukprn);
                    loadMetrics.NumberOfProvidersFailedDuringMapping++;
                }
                else
                {
                    await _courseDirectoryDataProcessingService.AugmentPilotData(provider);
                    
                    var successfulLoading = await _loadProvider.LoadProvider(provider);
                    if (successfulLoading)
                    {
                        _logger.LogInformation("Ukprn {ukprn} mapped and loaded successfully", provider.Ukprn);
                        loadMetrics.NumberOfProvidersLoadedSuccessfully++;
                    }
                    else
                    {
                        _logger.LogWarning("Ukprn {ukprn} failed to load", provider.Ukprn);
                        loadMetrics.NumberOfProvidersLoadedSuccessfully++;
                    }
                }
            }

            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, loadMetrics.NumberOfProvidersLoadedSuccessfully, ImportType.CourseDirectory));

            return loadMetrics;
        }

        private async Task CleanseDuplicateLarsCodes(CdProvider cdProvider,
            LarsCodeDuplicationMetrics larsCodeDuplicationMetrics, bool localRun)
        {
            var metrics = await _courseDirectoryDataProcessingService.CleanseDuplicateLarsCodes(cdProvider, localRun);
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
            var regions = await _regionsReadRepository.GetAllRegions();
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
            var standards = await _standardsReadRepository.GetAllStandards();
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
