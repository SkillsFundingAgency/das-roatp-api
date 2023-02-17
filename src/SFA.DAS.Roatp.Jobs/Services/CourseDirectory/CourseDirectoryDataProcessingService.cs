﻿using System;
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
        private readonly ILogger<CourseDirectoryDataProcessingService> _logger;

        public CourseDirectoryDataProcessingService(ILogger<CourseDirectoryDataProcessingService> logger, IProviderRegistrationDetailsReadRepository getActiveProviderRegistrationsRepository, IProvidersReadRepository providersReadRepository)
        {
            _getActiveProviderRegistrationsRepository = getActiveProviderRegistrationsRepository;
            _providersReadRepository = providersReadRepository;
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

        public Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(CdProvider provider)
        {
            var metrics = new LocationDuplicationMetrics();

            var locationMappingsSameNameAddress1Postcode = new List<LocationIdMapping>();

            var currentLocations = new List<CdProviderLocation>();
            var locationsToRemove = new List<CdProviderLocation>();
            var locationsWithSameNameDifferentAddress1Postcode = new List<CdProviderLocation>();
            foreach (var location in provider.Locations)
            {
                location.Name = location.Name?.Trim();
                if (!currentLocations.Select(x => x.Name).Contains(location.Name, StringComparer.OrdinalIgnoreCase))
                {
                    currentLocations.Add(location);
                }
                else
                {
                    var originalLocation = currentLocations.First(x =>
                        string.Equals(x.Name, location.Name, StringComparison.CurrentCultureIgnoreCase));

                    var locationMappingSameNameAddress1Postcode = new LocationIdMapping
                    {
                        OriginalId = originalLocation.Id,
                        DuplicateId = location.Id
                    };

                    if (originalLocation.Postcode == location.Postcode && originalLocation.AddressLine1 == location.AddressLine1)
                    {
                        locationMappingsSameNameAddress1Postcode.Add(locationMappingSameNameAddress1Postcode);
                        _logger.LogWarning("UKPRN:{ukprn},Course:, location:{id}, Duplicate location name/address,  Name: '{name}' |  Location type: '{locationType}' | address line 1: '{addressLine1}' | postcode: '{postcode}'",
                            provider.Ukprn, location.Id, location.Name.Trim(), location.LocationType, location.AddressLine1, location.Postcode);
                    }
                    else
                    {
                        locationsWithSameNameDifferentAddress1Postcode.Add(location);
                        _logger.LogWarning("UKPRN:{ukprn},Course:, location:{id}, Duplicate location name different address/Type,  Name: '{name}' | Location type: '{locationType}' | address line 1: '{addressLine1}' | postcode: '{postcode}'",
                            provider.Ukprn, location.Id, location.Name, location.LocationType, location.AddressLine1, location.Postcode);
                    }
                    locationsToRemove.Add(location);
                }
            }

            // remap any standards with name and address map to correct location
            foreach (var standard in provider.Standards)
            {
                foreach (var location in standard.Locations)
                {
                    foreach (var locationMapping in locationMappingsSameNameAddress1Postcode.Where(locationMapping => location.Id == locationMapping.DuplicateId))
                    {
                        if (standard.Locations.All(x => x.Id != locationMapping.OriginalId))
                        {
                            location.Id = locationMapping.OriginalId;
                        }
                    }
                }
            }

            if (locationsToRemove.Any())
            {
                metrics.ProvidersWithDuplicateLocationNames++;
                foreach (var locationToRemove in locationsToRemove)
                {
                    provider.Locations.Remove(locationToRemove);
                    metrics.ProviderLocationsRemoved++;
                }
            }
            
            return Task.FromResult(metrics);
        }

        public Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(CdProvider provider, bool localRun)
        {
            var metrics = new LarsCodeDuplicationMetrics();

            var coursesToSave = new List<CdProviderCourse>();
            var coursesToRemove = new List<CdProviderCourse>();
            foreach (var course in provider.Standards)
            {
                if (!coursesToSave.Select(x => x.StandardCode).Contains(course.StandardCode))
                {
                    coursesToSave.Add(course);
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
                    if (localRun)
                    {
                        foreach (var location in courseToRemove.Locations)
                        {
                            var delModes = string.Join(":", location.DeliveryModes);
                            _logger.LogWarning(
                                "UKPRN:{ukprn},Course:{standardCode}, location:{id}, Removing duplicate larsCode - location with deliverymodes '{deliveryModes}'",
                                provider.Ukprn, courseToRemove.StandardCode, location.Id, delModes);
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            "UKPRN:{ukprn},Course:{standardCode}, location:, Removing duplicate larsCode",
                            provider.Ukprn, courseToRemove.StandardCode);
                    }

                    provider.Standards.Remove(courseToRemove);
                    metrics.ProviderStandardsRemoved++;
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
                        _logger.LogWarning(
                            "UKPRN:{ukprn},Course: {StandardCode}, location:{id}, Unmatched ProviderCourseLocation, ", cdProvider.Ukprn, cdProviderCourse.StandardCode, courseLocation.Id);
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

            _logger.LogWarning(
                "UKPRN:,Course:, location:{id}, Unmatched Region name, Name: {name}  addressline1: {addressLine1}, Postcode {postcode}, LocationType {locationType}",
                cdProviderLocation.Id, cdProviderLocation.Name, cdProviderLocation.AddressLine1,
                cdProviderLocation.Postcode, cdProviderLocation.LocationType);
            return false;
        }

        private bool CheckStandardIsSuitable( Standard standard, int cdProviderCourseStandardCode, int cdProviderUkprn)
        {
            if (standard?.LarsCode == null || standard.LarsCode == 0)
            {
                _logger.LogWarning(
                    "UKPRN: {ukprn}, Course: {StandardCode}, location:, Unmatched StandardCode, no match in courses-api for StandardCode", cdProviderUkprn, cdProviderCourseStandardCode);
                return false;
            }

            return true;
        }

        public class LocationIdMapping
        {
            public int OriginalId { get; set; }
            public int DuplicateId { get; set; }
        }
    }
}