using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class LoadCourseDirectoryDataService: ILoadCourseDirectoryDataService
    {
        private readonly IGetCourseDirectoryDataService _getCourseDirectoryDataService;
        private readonly IGetActiveProviderRegistrationsRepository _getActiveProviderRegistrationsRepository;

        private readonly RoatpDataContext _roatpDataContext;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILoadProvidersFromCourseDirectoryRepository _loadProvidersFromCourseDirectoryRepository;
        private readonly ILogger<LoadCourseDirectoryDataService> _logger;

        // put in class???
        const decimal NationalLatitude = (decimal)52.564269;
        const decimal NationalLongitude = (decimal)-1.466056;

        // put in class
        const string DayRelease = "DayRelease";
        const string BlockRelease = "BlockRelease";
        private const string National = "National";

        public LoadCourseDirectoryDataService(IGetCourseDirectoryDataService getCourseDirectoryDataService, IProviderReadRepository providerReadRepository,  ILoadProvidersFromCourseDirectoryRepository loadProvidersFromCourseDirectoryRepository, ILogger<LoadCourseDirectoryDataService> logger, RoatpDataContext roatpDataContext, IGetActiveProviderRegistrationsRepository getActiveProviderRegistrationsRepository)
        {
            _getCourseDirectoryDataService = getCourseDirectoryDataService;
            _providerReadRepository = providerReadRepository;
            _loadProvidersFromCourseDirectoryRepository = loadProvidersFromCourseDirectoryRepository;
            _logger = logger;
            _roatpDataContext = roatpDataContext;
            _getActiveProviderRegistrationsRepository = getActiveProviderRegistrationsRepository;
        }

        public async Task LoadCourseDirectoryData()
        {
            var cdProviders = await _getCourseDirectoryDataService.GetCourseDirectoryData();
           


            // // get current list of providerRegistrationDetails (cached from roatp-service), and filter out non-active registrations
            // // put into a repository rather than direct call
            // ///  repository-- GET ACTIVE PROVIDERS 
            // var providerRegistrationDetails = _roatpDataContext.ProviderRegistrationDetails;
            // _logger.LogInformation($"Retrieved {providerRegistrationDetails.Count()} provider registration details from ProviderRegistrationDetail");
            //
            // //var activeProviderUkprns = providerRegistrationDetails.Where(x =>
            // //    x.StatusId == OrganisationStatus.Active ||
            // //    x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices).Select(x => x.Ukprn);
            //
            // //_logger.LogInformation($"Finding {activeProviderUkprns?.Count()} active provider ukprns");
            var activeProviders = await _getActiveProviderRegistrationsRepository.GetActiveProviderRegistrations();
            
            // remove any providers that are not on the roatp active providers lists
            cdProviders.RemoveAll(x => !activeProviders.Select(x=>x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{cdProviders.Count} CD providers after removing non-active organisations from the CD providers");
            //////////////-------------------////////////////////


            // get current list of providers, to remove those from providers to be inserted
            var currentProviders = await _providerReadRepository.GetAll();
            // remove current providers
            cdProviders.RemoveAll(x => currentProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{cdProviders.Count} CD providers to insert after removing providers already present in roatp database");
            ///////////////////////----------------------//////////////////////

            // map providers to model we want
            var providers = MapCourseDirectoryProviders(cdProviders).ToList();
            _logger.LogInformation($"Mapped {providers.Count} providers from course directory");

            // write new providers to database
            var successfulLoad =
                await _loadProvidersFromCourseDirectoryRepository.LoadProvidersFromCourseDirectory(providers);
           
            _logger.LogInformation("Load providers from course directory successful: {sucessfulLoad}",successfulLoad);
        }



        //MFCMFC cheange save to import at the end of the mapping
        // place within a testable service
        private IEnumerable<Provider> MapCourseDirectoryProviders(List<CdProvider> cdProviders)
        {

            // add logs to any decision points

            // put in repository
            var standards = _roatpDataContext.Standards.ToList();
            _logger.LogInformation($"Retrieved {standards.Count} standards from Standard table");

            // put in repository
            var regions = _roatpDataContext.Regions.ToList();
            _logger.LogInformation($"Retrieved {standards.Count} regions from Region table");


            var providers = new List<Provider>();

            foreach (var cdProvider in cdProviders)
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
                    IsImported = true,
                    //HasConfirmedDetails = false, // This is being removed from the database/EF
                    //HasConfirmedLocations = false // This is being removed from the database/EF
                };


                //MFCMFC not needed as a variable
                var website = cdProvider.Website;

                var providerLocations = new List<ProviderLocation>();
                foreach (var cdProviderLocation in cdProvider.Locations)
                {
                    var locationType = LocationType.Provider;
                    var locationName = cdProviderLocation.Name;
                    var address = cdProviderLocation.Address;

                    //MFCMFC
                    // expand the address object to include this mapping stuff so it simplifies the mapping
                    if (address?.Address1 == locationName && address?.Address2 == null && address?.Town == null && address?.Postcode == null)
                    {
                        if (locationName == null && cdProviderLocation.Address?.Lat == NationalLatitude.ToString() && cdProviderLocation.Address?.Long == NationalLongitude.ToString())
                        {
                            locationType = LocationType.National;
                            locationName = National;
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

                    
                    Region region = null;

                    if (locationType == LocationType.Regional)
                    {
                        region = regions.FirstOrDefault(x => x.SubregionName == locationName);
                        
                        // if it's a regional field, this should always be present, not sure how to handle this if it happens
                        // if (region == null)
                        //     throw new InvalidDataException($"Region location cannot be mapped to {locationName}");
                    }

                    providerLocations.Add(new ProviderLocation
                    {
                        ImportedLocationId = cdProviderLocation.Id,
                        NavigationId = Guid.NewGuid(),
                        LocationName = locationName,
                        LocationType = locationType,
                        AddressLine1 = address?.Address1,
                        AddressLine2 = address?.Address2,
                        Town = address?.Town,
                        Postcode = address?.Postcode,
                        County = address?.County,
                        Latitude = latitude,
                        Longitude = longitude,
                        Email = cdProviderLocation.Email,
                        Website = cdProviderLocation.Website,
                        Phone = cdProviderLocation.Phone,
                        IsImported = true,
                        Region = region
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
                        _logger.LogWarning($"LarsCode {cdProviderCourse.StandardCode} found no match in couses-api");
                        continue;
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
                        //IfateReferenceNumber = null, // This is being removed from the database/EF
                        StandardInfoUrl = cdProviderCourse.StandardInfoUrl ?? "", 
                        ContactUsPhoneNumber = cdProviderCourse.Contact?.Phone,
                        ContactUsEmail = cdProviderCourse.Contact?.Email,
                        ContactUsPageUrl = cdProviderCourse.Contact?.ContactUsUrl,
                        IsImported = true,
                        //IsConfirmed = false,   // This is being removed from the database/EF
                        //HasNationalDeliveryOption = nationalDeliveryOption,   // This is being removed from the database/EF
                        //HasHundredPercentEmployerDeliveryOption = hundredPercentEmployer, // This is being removed from the database/EF
                        //---- OffersPortableFlexiJob = false //---- This is being ADDED to the database/EF.  
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
                            // need to check this doesn't happen in prod, and if it does, how to deal with it
                            // MFCMFC add a log warning
                            continue;
                        }

                        providerCourseLocations.Add(new ProviderCourseLocation
                        {
                            NavigationId = Guid.NewGuid(),
                            Course = newProviderCourse,
                            Location = providerLocation,
                            //Radius = courseLocation.Radius == null ? 0 : (decimal)courseLocation.Radius, // This is being removed from the database/EF
                            HasDayReleaseDeliveryOption = dayRelease,
                            HasBlockReleaseDeliveryOption = blockRelease,
                            //OffersPortableFlexiJob = false  // This is being removed from the database/EF
                            IsImported = true
                        });
                    }

                    newProviderCourse.Locations = providerCourseLocations;
                    providerCourses.Add(newProviderCourse);
                }

                provider.Courses = providerCourses;

                providers.Add(provider);
                //MFCMFC do not add to the providers, insert the record

            }

            return providers;
        }
    }
}
