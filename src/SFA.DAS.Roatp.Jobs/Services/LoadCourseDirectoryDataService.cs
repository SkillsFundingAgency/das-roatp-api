﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using Standard = SFA.DAS.Roatp.Jobs.ApiModels.Lookup.Standard;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class LoadCourseDirectoryDataService: ILoadCourseDirectoryDataService
    {
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<LoadCourseDirectoryDataService> _logger;

        // put in class???
        const decimal NationalLatitude = (decimal)52.564269;
        const decimal NationalLongitude = (decimal)-1.466056;
        const string HundredPercentEmployer = "100PercentEmployer";
        const string DayRelease = "DayRelease";
        const string BlockRelease = "BlockRelease";

        public LoadCourseDirectoryDataService(ICourseManagementOuterApiClient courseManagementOuterApiClient, ILogger<LoadCourseDirectoryDataService> logger)
        {
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _logger = logger;
        }

        public async Task LoadCourseDirectoryData()
        {
            var (success, courseDirectoryResponse) = await _courseManagementOuterApiClient.Get<string>("lookup/course-directory-data");

            if (!success)
            {
                const string errorMessage = "Unexpected response when trying to get course directory details from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            var cdProviders = JsonConvert.DeserializeObject<List<CdProvider>>(courseDirectoryResponse);
            _logger.LogInformation($"{cdProviders.Count} providers returned from Course Directory");



            // get latest standards from courses-api or use roatp cache????


            var (standardsSuccess, standardList) = await _courseManagementOuterApiClient.Get<StandardList>("lookup/standards");
            if (!standardsSuccess || !standardList.Standards.Any())
            {
                _logger.LogError($"ReloadStandardsCacheFunction function failed to get active standards");
                throw new InvalidOperationException("No standards were retrieved from courses api");
            }

            // use roatp cache for this???
           // var standardsToReload = standardList.Standards.Select(standard => (Domain.Entities.Standard)standard).ToList();

            // map providers to model we want
            var providers = MapCourseDirectoryProviders(cdProviders, standardList.Standards);


            

            // get roatp active providers

            // remove any providers that are not on the roatp active providers lists

            // get current list of providers, and remove those from providers?


            // write new providers to database


        }


        // this is best placed within a testable service
        private IEnumerable<Provider> MapCourseDirectoryProviders(List<CdProvider> cdProviders, IReadOnlyCollection<Standard> standards)
        {
            var providers = new List<Provider>();

            foreach (var cdProvider in cdProviders)
            {
                var provider = new Provider
                {
                    Ukprn = cdProvider.Ukprn,
                    LegalName = cdProvider.Name,
                    TradingName = cdProvider.TradingName,
                    Email = cdProvider.Email,
                    Website = cdProvider.Website,
                    Phone = cdProvider.Phone,
                    EmployerSatisfaction = cdProvider.EmployerSatisfaction,
                    LearnerSatisfaction = cdProvider.LearnerSatisfaction,
                    MarketingInfo = cdProvider.MarketingInfo,
                    HasConfirmedDetails = false,
                    HasConfirmedLocations = false
                };

                var providerLocations = new List<ProviderLocation>();
                foreach (var cdProviderLocation in cdProvider.Locations)
                {
                    var locationType = LocationType.Provider;
                    if (cdProviderLocation.Address?.Address1 == cdProviderLocation.Name && cdProviderLocation.Address?.Postcode == null)
                    {
                        if (cdProviderLocation.Name == null && cdProviderLocation.Address?.Lat == NationalLatitude.ToString() && cdProviderLocation.Address?.Long == NationalLongitude.ToString())
                        {
                            locationType = LocationType.National;
                        }
                        else
                        {
                            locationType = LocationType.Regional;
                        }
                    }

                    decimal? latitude = null;
                    decimal? longitude = null;

                    if (decimal.TryParse(cdProviderLocation.Address?.Lat, out decimal latitudeParsed))
                    {
                        latitude = latitudeParsed;
                    }

                    if (decimal.TryParse(cdProviderLocation.Address?.Long, out decimal longitudeParsed))
                    {
                        longitude = longitudeParsed;
                    }

                    if (cdProvider.Ukprn == 10000082)
                    {
                        var zzzz = 1;
                    }

                    providerLocations.Add(new ProviderLocation
                    {
                        NavigationId = Guid.NewGuid(),
                        ImportedLocationId = cdProviderLocation.Id,
                        LocationName = cdProviderLocation.Name ?? (locationType == LocationType.National ? "National" : "No name given"),
                        LocationType = locationType,
                        AddressLine1 = cdProviderLocation.Address?.Address1,
                        AddressLine2 = cdProviderLocation.Address?.Address2,
                        County = cdProviderLocation.Address?.County,
                        Town = cdProviderLocation.Address?.Town,
                        Postcode = cdProviderLocation.Address?.Postcode,
                        Latitude = latitude,
                        Longitude = longitude,
                        IsImported = true,
                        Email = cdProviderLocation.Email,
                        Website = cdProviderLocation.Website,
                        Phone = cdProviderLocation.Phone
                    });
                }

                provider.Locations = providerLocations;

                var providerCourses = new List<ProviderCourse>();

                
                foreach (var cdProviderCourse in cdProvider.Standards)
                {

                    //What shall I do if no standard returned???  For now, doing a break
                    var standard = standards.FirstOrDefault(x => x.LarsCode == cdProviderCourse.StandardCode);

                    if (standard?.LarsCode == null || standard.LarsCode == 0)
                    {
                        // jump out? exception? Log?
                        _logger.LogError($"LarsCode {cdProviderCourse.StandardCode} found no match in couses-api");
                        break;
                    }
                    
                    // var nationalDeliveryOption = false;
                    //
                    // foreach (var courseLocation in cdProviderCourse.Locations)
                    // {
                    //     var matchingLocation =
                    //         providerLocations.FirstOrDefault(x => x.ImportedLocationId == courseLocation.Id);
                    //     if (matchingLocation is { LocationType: LocationType.National })
                    //     {
                    //         nationalDeliveryOption = true;
                    //         break;
                    //     }
                    // }
                    //
                    // var hundredPercentEmployer = false;
                    // foreach (var location in cdProviderCourse.Locations)
                    // {
                    //     if (location.DeliveryModes.Any(deliveryMode => deliveryMode == HundredPercentEmployer))
                    //     {
                    //         hundredPercentEmployer = true;
                    //         break;
                    //     }
                    // }

                    var newProviderCourse = new ProviderCourse
                    {
                        LarsCode = cdProviderCourse.StandardCode,
                        IfateReferenceNumber = null, // this is being retired, ignore
                        StandardInfoUrl = cdProviderCourse.StandardInfoUrl,
                        ContactUsPhoneNumber = cdProviderCourse.Contact?.Phone,
                        ContactUsEmail = cdProviderCourse.Contact?.Email,
                        ContactUsPageUrl = cdProviderCourse.Contact?.ContactUsUrl,
                        IsImported = true,
                        //IsConfirmed = false,
                        //HasNationalDeliveryOption = nationalDeliveryOption,
                        //HasHundredPercentEmployerDeliveryOption = hundredPercentEmployer,
                        Versions = new List<ProviderCourseVersion>
                    {
                        new ProviderCourseVersion
                        {
                            // what to do if no standard returned?
                            StandardUId = standard.StandardUId,
                            Version = standard.Version
                        }
                    }
                    };


                    var providerCourseLocations = new List<ProviderCourseLocation>();

                    foreach (var courseLocation in cdProviderCourse.Locations)
                    {

                        var blockRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == BlockRelease);
                        var dayRelease = courseLocation.DeliveryModes.Any(deliveryMode => deliveryMode == DayRelease);

                        var providerLocation = provider.Locations.FirstOrDefault(x => x.ImportedLocationId == courseLocation.Id);

                        if (providerLocation == null)
                        {
                            // there are numerous cases of the provider.location not existing for the providercourselocation id
                            break;
                        }

                        providerCourseLocations.Add(new ProviderCourseLocation
                        {
                            NavigationId = Guid.NewGuid(),
                            Course = newProviderCourse,
                            Location = providerLocation,
                            //Radius = courseLocation.Radius == null ? 0 : (decimal)courseLocation.Radius, // To delete?
                            HasDayReleaseDeliveryOption = dayRelease,
                            HasBlockReleaseDeliveryOption = blockRelease,
                            IsImported = true,
                            //OffersPortableFlexiJob = false  // default value on entry????
                        });
                    }

                    newProviderCourse.Locations = providerCourseLocations;
                    providerCourses.Add(newProviderCourse);
                }

                provider.Courses = providerCourses;

                providers.Add(provider);

            }

            return providers;
        }

    }
}
