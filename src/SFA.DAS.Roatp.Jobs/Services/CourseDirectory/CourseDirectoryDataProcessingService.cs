using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services.Metrics;
using Standard = SFA.DAS.Roatp.Domain.Entities.Standard;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    [ExcludeFromCodeCoverage]
    public class CourseDirectoryDataProcessingService : ICourseDirectoryDataProcessingService
    {
        private readonly IProviderRegistrationDetailsReadRepository _getActiveProviderRegistrationsRepository;
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly IGetBetaProvidersService _getBetaProvidersService;
        private readonly ILogger<CourseDirectoryDataProcessingService> _logger;

        public CourseDirectoryDataProcessingService(ILogger<CourseDirectoryDataProcessingService> logger, IProviderRegistrationDetailsReadRepository getActiveProviderRegistrationsRepository, IProvidersReadRepository providersReadRepository, IGetBetaProvidersService getBetaProvidersService)
        {
            _getActiveProviderRegistrationsRepository = getActiveProviderRegistrationsRepository;
            _providersReadRepository = providersReadRepository;
            _getBetaProvidersService = getBetaProvidersService;
            _logger = logger;
        }

        public async Task<int> RemoveProvidersNotActiveOnRegister(List<CdProvider> providers)
        {
            const string focusText = "active registered providers from roatp-service cache";
            _logger.LogInformation("Gathering {focus}",focusText);
            var activeProviders = await _getActiveProviderRegistrationsRepository.GetActiveProviderRegistrations();
            _logger.LogInformation("{count} {focus}",activeProviders.Count, focusText);
            _logger.LogInformation("{count} CD providers before removing non-{focus}", providers.Count, focusText);
           
            providers.RemoveAll(x => !activeProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation("{count} CD providers after removing non-{focus}", providers.Count, focusText);
            return activeProviders.Count;
        }

        public async Task<int> RemovePreviouslyLoadedProviders(List<CdProvider> providers)
        {
            const string focusText = "providers already present in roatp database";
            _logger.LogInformation("Gathering {focus}", focusText);
            var currentProviders = await _providersReadRepository.GetAllProviders();
            _logger.LogInformation("{count} {focus}", currentProviders.Count, focusText);
            _logger.LogInformation("{count} CD providers before removing {focus}", providers.Count, focusText);

            providers.RemoveAll(x => currentProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation("{count} CD providers to insert after removing {focus}", providers.Count, focusText);
            return currentProviders.Count;
        }

        public Task<BetaAndPilotProviderMetrics> RemoveProvidersNotOnBetaOrPilotList(List<CdProvider> providers)
        { 
            var metrics = new BetaAndPilotProviderMetrics();
            const string focusText = "beta and pilot providers";

            var betaProviders = _getBetaProvidersService.GetBetaProviderUkprns();

            var betaAndPilotUkprns = new List<int>();
            betaAndPilotUkprns.AddRange(betaProviders);
            betaAndPilotUkprns.AddRange(PilotProviders.Ukprns);

            metrics.PilotProviders = PilotProviders.Ukprns.Count;
            metrics.BetaProviders = betaProviders.Count;

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
                if (!currentLocationNames.Contains(location.Name?.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    currentLocationNames.Add(location.Name?.Trim());
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
                    _logger.LogWarning("Duplicate location name - provider UKPRN {ukprn}: removing location id {id} location name '{name}'",provider.Ukprn,locationToRemove.Id,locationToRemove.Name.Trim());
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
                foreach (var larsCode in PilotProviderCourses.LarsCodes)
                {
                    if (provider.Courses.All(x => x.LarsCode != larsCode))
                    {
                        provider.Courses.Add(new Domain.Entities.ProviderCourse { LarsCode = larsCode, HasPortableFlexiJobOption = true });
                        _logger.LogInformation("Adding pilot courses for UKPRN {ukprn} LarsCode {LarsCode}", provider.Ukprn, larsCode);
                    }
                    else
                    {
                        provider.Courses.First(x => x.LarsCode == larsCode).HasPortableFlexiJobOption = true;
                    }
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

            foreach (var cdProviderLocation in cdProvider.Locations)
            {
                int? regionId = null;
                var regionIdMapped = RegionIdMapped(cdProviderLocation, regions, ref regionId);

                if(!regionIdMapped)
                    return Task.FromResult((false, (Provider)null));

                AddProviderLocation(provider, cdProviderLocation, regionId);
            }

            foreach (var cdProviderCourse in cdProvider.Standards)
            {
                var standard = standards.FirstOrDefault(x => x.LarsCode == cdProviderCourse.StandardCode);

                if(!CheckStandardIsSuitable(standard, cdProviderCourse.StandardCode, cdProvider.Ukprn))
                    break;

                var newProviderCourse = new Domain.Entities.ProviderCourse
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
                    
                    if (providerLocation == null)
                    {
                        //PRODCHECK
                        // there are numerous cases of the provider.location not existing for the providercourselocation id
                        // it seems reasonable to continue as the remaining data is coherent
                        // need to check this doesn't happen in prod, and if it does, how to deal with it
                        _logger.LogWarning("Provider course location id {id} found no match in provider location for ukprn: {ukprn}", courseLocation.Id, cdProvider.Ukprn);
                    }
                    else
                    {
                        var blockRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.BlockRelease);
                        var dayRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DeliveryMode.DayRelease);
                        providerCourseLocations.Add(new ProviderCourseLocation
                        {
                            NavigationId = Guid.NewGuid(),
                            ProviderCourse = newProviderCourse,
                            Location = providerLocation,
                            HasDayReleaseDeliveryOption = dayRelease,
                            HasBlockReleaseDeliveryOption = blockRelease,
                            IsImported = true
                        });
                    }
                }

                newProviderCourse.Locations = providerCourseLocations;
                provider.Courses.Add(newProviderCourse);
            }

            return Task.FromResult((true, provider));
        }

        private static void AddProviderLocation(Provider provider, CdProviderLocation cdProviderLocation, int? regionId)
        {
            provider.Locations.Add(new ProviderLocation
            {
                ImportedLocationId = cdProviderLocation.Id,
                NavigationId = Guid.NewGuid(),
                LocationName = cdProviderLocation.LocationType == LocationType.Provider ? cdProviderLocation.Name : null,
                LocationType = cdProviderLocation.LocationType,
                AddressLine1 =
                    cdProviderLocation.LocationType == LocationType.Provider ? cdProviderLocation.AddressLine1 : null,
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

        private bool RegionIdMapped(CdProviderLocation cdProviderLocation, IEnumerable<Region> regions, ref int? regionId)
        {
            if (cdProviderLocation.LocationType != LocationType.Regional) return true;
         
            regionId = regions.FirstOrDefault(x => x.SubregionName == cdProviderLocation.Name)?.Id;
            if (regionId != null) return true;
    
            _logger.LogWarning("Region location cannot be mapped to {name}", cdProviderLocation.Name);
            return false;
        }

        private bool CheckStandardIsSuitable( Standard standard, int cdProviderCourseStandardCode, int cdProviderUkprn)
        {
            if (standard?.LarsCode == null || standard.LarsCode == 0)
            {
                _logger.LogWarning("LarsCode {standardCode} for ukprn {ukprn} found no match in courses-api",
                    cdProviderCourseStandardCode, cdProviderUkprn);
                return false;
            }

            return true;
        }
    }
}