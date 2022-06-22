using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services.Metrics;
using Standard = SFA.DAS.Roatp.Domain.Entities.Standard;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class CourseDirectoryDataProcessingService : ICourseDirectoryDataProcessingService
    {
        private readonly IGetActiveProviderRegistrationsRepository _getActiveProviderRegistrationsRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<CourseDirectoryDataProcessingService> _logger;
        private const string National = "National";


        public CourseDirectoryDataProcessingService(ILogger<CourseDirectoryDataProcessingService> logger, IGetActiveProviderRegistrationsRepository getActiveProviderRegistrationsRepository, IProviderReadRepository providerReadRepository)
        {
            _getActiveProviderRegistrationsRepository = getActiveProviderRegistrationsRepository;
            _providerReadRepository = providerReadRepository;
            _logger = logger;
        }

        public async Task RemoveProvidersNotActiveOnRegister(List<CdProvider> providers)
        {
            var focusText = "active registered providers from roatp-service cache";
            _logger.LogInformation($"Gathering {focusText}");
            var activeProviders = await _getActiveProviderRegistrationsRepository.GetActiveProviderRegistrations();
            _logger.LogInformation($"{activeProviders.Count} {focusText}");
            _logger.LogInformation($"{providers.Count} CD providers before removing non-{focusText}");
           
            providers.RemoveAll(x => !activeProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers after removing non-{focusText}");

        }

        public async Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers)
        {
            var focusText = "providers already present in roatp database";
            _logger.LogInformation($"Gathering {focusText}");
            var currentProviders = await _providerReadRepository.GetAllProviders();
            _logger.LogInformation($"{currentProviders.Count} {focusText}");
            _logger.LogInformation($"{providers.Count} CD providers before removing {focusText}");

            providers.RemoveAll(x => currentProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers to insert after removing {focusText}");
        }

        public async Task RemoveProvidersNotOnBetaList(List<CdProvider> providers)
        {
            var focusText = "beta providers";
           
            _logger.LogInformation($"{providers.Count} CD providers before removing non-{focusText}");
            providers.RemoveAll(x => !BetaProviders.Ukprns.Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers to insert after removing non-{focusText}");
        }

        public async Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(CdProvider provider)
        {
            var metrics = new LocationDuplicationMetrics();

          
            var currentLocationNames = new List<string>();
            var locationsToRemove = new List<CdProviderLocation>();
            foreach (var location in provider.Locations)
            {
                if (!currentLocationNames.Contains(location.Name))
                {
                    currentLocationNames.Add(location.Name);
                }
                else
                {
                    locationsToRemove.Add(location);
                }
            }

            if (locationsToRemove.Any())
            {
                metrics.ProvidersWithDuplicateLocationNames++;
                foreach (var locationToRemove in locationsToRemove)
                {
                    provider.Locations.Remove(locationToRemove);
                    metrics.ProviderLocationsRemoved++;
                    _logger.LogWarning($"Duplicate location name - provider UKPRN {provider.Ukprn}: removing location id {locationToRemove.Id} location name '{locationToRemove.Name}'");
                }
            }
            
            return metrics;
        }

        public async Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(CdProvider provider)
        {
            var metrics = new LarsCodeDuplicationMetrics();

           
            var currentLarsCodes = new List<int>();
            var coursesToRemove = new List<CdProviderCourse>();
            foreach (var course in provider.Standards)
            {
                if (!currentLarsCodes.Contains(course.StandardCode))
                {
                    currentLarsCodes.Add(course.StandardCode);
                }
                else
                {
                    coursesToRemove.Add(course);
                }
            }

            if (coursesToRemove.Any())
            {
                metrics.ProvidersWithDuplicateStandards++;
                foreach (var courseToRemove in coursesToRemove)
                {
                    provider.Standards.Remove(courseToRemove);
                    metrics.ProviderStandardsRemoved++;
                    _logger.LogWarning($"Duplicate lars code - provider UKPRN {provider.Ukprn}: removing duplicate larsCode {courseToRemove.StandardCode}'");
                }
            }
            
            return metrics;
        }

        public async Task InsertMissingPilotData(CdProvider provider)
        {
            if (PilotProviders.Ukprns.All(x => x != provider.Ukprn))
            {
                return;
            }

            foreach (var pilotCourse in PilotProviderCourses.PilotCourses.Where(pilotCourse => provider.Standards.All(x => x.StandardCode != pilotCourse.LarsCode)))
            {
                provider.Standards.Add(new CdProviderCourse {StandardCode = pilotCourse.LarsCode, StandardInfoUrl = pilotCourse.StandardInfoUrl});
                _logger.LogInformation($"Adding pilot courses for UKPRN {provider.Ukprn} LarsCode {pilotCourse.LarsCode}");
            }
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
                    LocationName = cdProviderLocation.LocationType != LocationType.National ? cdProviderLocation.Name : National,
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
            
            var isPilotProvider = PilotProviders.Ukprns.Contains(cdProvider.Ukprn);

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
                    HasPortableFlexiJobOption = isPilotProvider,
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