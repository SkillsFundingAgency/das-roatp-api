using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
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
            const string focusText = "active registered providers from roatp-service cache";
            _logger.LogInformation("Gathering {focus}",focusText);
            var activeProviders = await _getActiveProviderRegistrationsRepository.GetActiveProviderRegistrations();
            _logger.LogInformation("{count} {focus}",activeProviders.Count, focusText);
            _logger.LogInformation("{count} CD providers before removing non-{focus}", providers.Count, focusText);
           
            providers.RemoveAll(x => !activeProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation("{count} CD providers after removing non-{focus}", providers.Count, focusText);
        }

        public async Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers)
        {
            const string focusText = "providers already present in roatp database";
            _logger.LogInformation("Gathering {focus}", focusText);
            var currentProviders = await _providerReadRepository.GetAllProviders();
            _logger.LogInformation("{count} {focus}", currentProviders.Count, focusText);
            _logger.LogInformation("{count} CD providers before removing {focus}", providers.Count, focusText);

            providers.RemoveAll(x => currentProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation("{count} CD providers to insert after removing {focus}", providers.Count, focusText);
        }

        public  Task<BetaAndPilotProviderMetrics> RemoveProvidersNotOnBetaOrPilotList(List<CdProvider> providers)
        {
            var metrics = new BetaAndPilotProviderMetrics();
            const string focusText = "beta and pilot providers";

            var betaAndPilotUkprns = BetaProviders.Ukprns;
            betaAndPilotUkprns.AddRange(PilotProviders.Ukprns);

            metrics.PilotProviders = PilotProviders.Ukprns.Count;
            metrics.BetaProviders = BetaProviders.Ukprns.Count;
            metrics.CombinedBetaAndPilotProvidersProcessed = betaAndPilotUkprns.Distinct().Count();

            _logger.LogInformation("{count} CD providers before removing non-{focus}", providers.Count, focusText);
            providers.RemoveAll(x => !betaAndPilotUkprns.Distinct().Contains(x.Ukprn));
            _logger.LogInformation("{count} CD providers to insert after removing non-{focus}",providers.Count, focusText);

            return Task.FromResult(metrics);
        }

        public Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(CdProvider provider)
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
                    _logger.LogWarning("Duplicate location name - provider UKPRN {ukprn}: removing location id {id} location name '{name}'",provider.Ukprn,locationToRemove.Id,locationToRemove.Name);
                }
            }
            
            return Task.FromResult(metrics);
        }

        public Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(CdProvider provider)
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
                    _logger.LogWarning(
                        "Duplicate lars code - provider UKPRN {ukprn}: removing duplicate larsCode {standardCode}'",
                        provider.Ukprn, courseToRemove.StandardCode);
                }
            }

            return Task.FromResult(metrics); 
        }

        public Task AugmentPilotData(Provider provider)
        {
            if (PilotProviders.Ukprns.Any(x => x == provider.Ukprn))
            {
                foreach (var larsCode in PilotProviderCourses.LarsCodes.Where(l => provider.Courses.All(x => x.LarsCode != l)))
                {
                    provider.Courses.Add(new ProviderCourse { LarsCode = larsCode, HasPortableFlexiJobOption = true });
                    _logger.LogInformation("Adding pilot courses for UKPRN {ukprn} LarsCode {LarsCode}", provider.Ukprn, larsCode);
                }
            }

            return Task.CompletedTask;
        }

        public Task<(bool, Provider)> MapCourseDirectoryProvider(CdProvider cdProvider, List<Standard> standards, List<Region> regions)
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
                        _logger.LogWarning("Region location cannot be mapped to {name}",cdProviderLocation.Name);
                        return Task.FromResult((false, (Provider)null));
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
            
            foreach (var cdProviderCourse in cdProvider.Standards)
            {
                var standard = standards.FirstOrDefault(x => x.LarsCode == cdProviderCourse.StandardCode);

                if (CheckStandardLarsCode( standard,cdProviderCourse.StandardCode, cdProvider.Ukprn)) break;

                var newProviderCourse = new ProviderCourse
                {
                    LarsCode = cdProviderCourse.StandardCode,
                    StandardInfoUrl = cdProviderCourse.StandardInfoUrl,
                    ContactUsPhoneNumber = cdProviderCourse.ContactUsPhoneNumber,
                    ContactUsEmail = cdProviderCourse.ContactUsEmail,
                    ContactUsPageUrl = cdProviderCourse.ContactUsPageUrl,
                    IsImported = true,
                    HasPortableFlexiJobOption = false,
                    Versions = new List<ProviderCourseVersion>
                    {
                        new ProviderCourseVersion
                        {
                            StandardUId = standard?.StandardUId,
                            Version = standard?.Version
                        }
                    }
                };

                var providerCourseLocations = new List<ProviderCourseLocation>();

                foreach (var courseLocation in cdProviderCourse.Locations)
                {
                  
                    var providerLocation = provider.Locations.FirstOrDefault(x => x.ImportedLocationId == courseLocation.Id);
                    AddProviderLocation(providerCourseLocations, providerLocation,  newProviderCourse, courseLocation,  cdProvider.Ukprn );
                }

                newProviderCourse.Locations = providerCourseLocations;
                providerCourses.Add(newProviderCourse);
            }

            provider.Courses = providerCourses;

            return Task.FromResult((true, provider));
        }

        private void AddProviderLocation(ICollection<ProviderCourseLocation> providerCourseLocations, ProviderLocation providerLocation, ProviderCourse newProviderCourse,
            CdProviderCourseLocation courseLocation, int cdProviderUkprn)
        {
            if (providerLocation == null)
            {
                //PRODCHECK
                // there are numerous cases of the provider.location not existing for the providercourselocation id
                // it seems reasonable to continue as the remaining data is coherent
                // need to check this doesn't happen in prod, and if it does, how to deal with it
                _logger.LogWarning("Provider course location id {id} found no match in provider location for ukprn: {ukprn}",
                    courseLocation.Id, cdProviderUkprn);
            }
            else
            {
                var blockRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.BlockRelease);
                var dayRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.DayRelease);

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

        private bool CheckStandardLarsCode( Standard standard, int cdProviderCourseStandardCode, int cdProviderUkprn)
        {
            if (standard?.LarsCode != null && standard.LarsCode != 0) return false;
            _logger.LogWarning("LarsCode {standardCode} for ukprn {ukprn} found no match in courses-api",
                cdProviderCourseStandardCode, cdProviderUkprn);
            return true;

        }
    }
}