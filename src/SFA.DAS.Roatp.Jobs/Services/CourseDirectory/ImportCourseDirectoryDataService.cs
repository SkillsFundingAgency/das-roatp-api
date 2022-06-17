using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class ImportCourseDirectoryDataService: IImportCourseDirectoryDataService
    {
        private readonly ILogger<ImportCourseDirectoryDataService> _logger;
        private readonly IStandardReadRepository _standardReadReadRepository;
        private readonly IRegionReadRepository _regionReadRepository;
        private readonly ILoadProviderFromCourseDirectoryRepository _loadProviderFromCourseDirectory;

        private const string National = "National";

        public ImportCourseDirectoryDataService(ILogger<ImportCourseDirectoryDataService> logger, IStandardReadRepository standardReadReadRepository, IRegionReadRepository regionReadRepository, ILoadProviderFromCourseDirectoryRepository loadProviderFromCourseDirectory)
        {
            _logger = logger;
            _standardReadReadRepository = standardReadReadRepository;
            _regionReadRepository = regionReadRepository;
            _loadProviderFromCourseDirectory = loadProviderFromCourseDirectory;
        }


        public async Task<CourseDirectoryImportMetrics> ImportCourseDirectoryData(List<CdProvider> cdProviders)
        {
            var metrics = new CourseDirectoryImportMetrics();
            
            var standards = await _standardReadReadRepository.GetAllStandards();
            if (standards == null || standards.Count == 0)
            {
                var errorMessage = "No standards could be retrieved from the standards cache";
                _logger.LogError(errorMessage);
                throw new InvalidDataException(errorMessage);
            }
            _logger.LogInformation($"Retrieved {standards?.Count} standards from Standard table");

            var regions = await _regionReadRepository.GetAllRegions();
            if (standards == null || standards.Count == 0)
            {
                var errorMessage = "No regionss could be retrieved from the regions table";
                _logger.LogError(errorMessage);
                throw new InvalidDataException(errorMessage);
            }
            _logger.LogInformation($"Retrieved {regions.Count} regions from Region table");

            metrics.ProvidersToLoad = cdProviders.Count;

            foreach (var cdProvider in cdProviders)
            {
                var (successMapping, provider) = await MapCourseDirectoryProvider(cdProvider,standards,regions);
                if (successMapping)
                {
                    var successfulLoading = await _loadProviderFromCourseDirectory.LoadProviderFromCourseDirectory(provider);
                    if (successfulLoading)
                    {
                        _logger.LogInformation($"Ukprn {cdProvider.Ukprn} mapped and loaded successfully");
                        metrics.SuccessfulLoads++;
                    }
                    else
                    {
                        _logger.LogWarning($"Ukprn {cdProvider.Ukprn} failed to load");
                        metrics.FailedLoads++;
                    }
                }
                else
                {
                    _logger.LogWarning($"Ukprn {cdProvider.Ukprn} failed to map");
                    metrics.FailedMappings++;
                }
            }

            return metrics;
        }

       
        public async Task<(bool, Provider)> MapCourseDirectoryProvider(CdProvider cdProvider, List<Standard> standards, List<Region> regions)
        {
            var provider = new Provider
            {
                Ukprn = cdProvider.Ukprn,
                LegalName = cdProvider.Name,
                TradingName = cdProvider.TradingName,
                Email = cdProvider.Email,
                Phone = cdProvider.Phone,
                Website = cdProvider.Website,
                MarketingInfo = cdProvider.MarketingInfo,
                EmployerSatisfaction = cdProvider.EmployerSatisfaction,
                LearnerSatisfaction = cdProvider.LearnerSatisfaction,
                IsImported = true
            };

            var providerLocations = new List<ProviderLocation>();
            foreach (var cdProviderLocation in cdProvider.Locations)
            {
                
                int? regionId = null;

                if (cdProviderLocation.LocationType == LocationType.Regional)
                {
                    regionId = regions.FirstOrDefault(x => x.SubregionName == cdProviderLocation.Name)?.Id;
                    if (regionId == null)
                    {
                        var errorMessage = $"Region location cannot be mapped to {cdProviderLocation.Name}";
                        _logger.LogWarning(errorMessage);
                        return (false, null);
                    }
                }

                providerLocations.Add(new ProviderLocation
                {
                    ImportedLocationId = cdProviderLocation.Id,
                    NavigationId = Guid.NewGuid(),
                    LocationName = cdProviderLocation.LocationType!=LocationType.National? cdProviderLocation.Name : National,
                    LocationType = cdProviderLocation.LocationType,
                    AddressLine1 = cdProviderLocation.AddressLine1,
                    AddressLine2 = cdProviderLocation.AddressLine2,
                    Town = cdProviderLocation.Town,
                    Postcode = cdProviderLocation.Postcode,
                    County = cdProviderLocation.County,
                    Latitude = cdProviderLocation.Latitude,
                    Longitude = cdProviderLocation.Longitude,
                    Email = cdProviderLocation.Email,
                    Website = cdProviderLocation.Website,
                    Phone = cdProviderLocation.Phone,
                    IsImported = true,
                    RegionId = regionId
                });
            }

            provider.Locations = providerLocations;

            var providerCourses = new List<ProviderCourse>();

            foreach (var cdProviderCourse in cdProvider.Standards)
            {
                var standard = standards.FirstOrDefault(x => x.LarsCode == cdProviderCourse.StandardCode);

                if (standard?.LarsCode == null || standard.LarsCode == 0)
                {
                    var errorMessage = $"LarsCode {cdProviderCourse.StandardCode} for ukprn {cdProvider.Ukprn} found no match in courses-api";
                    _logger.LogWarning(errorMessage);
                    break;
                }


                var newProviderCourse = new ProviderCourse
                {
                    LarsCode = cdProviderCourse.StandardCode,
                    StandardInfoUrl = cdProviderCourse.StandardInfoUrl ?? "",
                    ContactUsPhoneNumber = cdProviderCourse.ContactUsPhoneNumber,
                    ContactUsEmail = cdProviderCourse.ContactUsEmail,
                    ContactUsPageUrl = cdProviderCourse.ContactUsPageUrl,
                    IsImported = true,
                    //---- OffersPortableFlexiJob = false //---- This is being ADDED to the database/EF.  MFC
                    Versions = new List<ProviderCourseVersion>
                    {
                        new ProviderCourseVersion
                        {
                            StandardUId = standard.StandardUId,
                            Version = standard.Version
                        }
                    }
                };


                var providerCourseLocations = new List<ProviderCourseLocation>();

                foreach (var courseLocation in cdProviderCourse.Locations)
                {
                    var blockRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.BlockRelease);
                    var dayRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.DayRelease);

                    var providerLocation = provider.Locations.FirstOrDefault(x => x.ImportedLocationId == courseLocation.Id);

                    if (providerLocation == null)
                    {
                        //PRODCHECK
                        // there are numerous cases of the provider.location not existing for the providercourselocation id
                        // it seems reasonable to continue as the remaining data is coherent
                        // need to check this doesn't happen in prod, and if it does, how to deal with it
                        var warningMessage =
                            $"Provider course location id {courseLocation.Id} found no match in provider location for ukprn: {cdProvider.Ukprn}";
                        _logger.LogWarning(warningMessage);
                    }
                    else
                    {
                        providerCourseLocations.Add(new ProviderCourseLocation
                        {
                            NavigationId = Guid.NewGuid(),
                            Course = newProviderCourse,
                            Location = providerLocation,
                            HasDayReleaseDeliveryOption = dayRelease,
                            HasBlockReleaseDeliveryOption = blockRelease,
                            IsImported = true
                        });
                    }
                }

                newProviderCourse.Locations = providerCourseLocations;
                providerCourses.Add(newProviderCourse);
            }

            provider.Courses = providerCourses;

            return (true, provider);
        }
    }
}