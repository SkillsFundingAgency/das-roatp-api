using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Services
{
    public class ProcessProviderCourseLocationsService : IProcessProviderCourseLocationsService
    {
        public List<DeliveryModel> ConvertProviderLocationsToDeliveryModels(List<ProviderCourseLocationDetailsModel> providerCourseLocations)
        {
            var deliveryModels = new List<DeliveryModel>();

            if (providerCourseLocations == null || !providerCourseLocations.Any())
            {
                deliveryModels.Add(
                    new DeliveryModel
                    {
                        DeliveryModeType = DeliveryModeType.NotFound
                    });

                return deliveryModels;
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.National))
            {
                deliveryModels.Add(
                    new DeliveryModel
                    {
                        DeliveryModeType = DeliveryModeType.National
                    });
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.Regional))
            {
                var nearestRegionalLocation = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Regional).OrderBy(x => x.Distance).First();
                
                deliveryModels.Add(
                    new DeliveryModel
                    {
                        DeliveryModeType = DeliveryModeType.Workplace,
                        DistanceInMiles = nearestRegionalLocation.Distance
                    });
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.Provider))
            {
                var nearestLocationDayRelease = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Provider && l.HasDayReleaseDeliveryOption == true).MinBy(l => l.Distance);

                if (nearestLocationDayRelease != null)
                    deliveryModels.Add(
                    new DeliveryModel
                    {
                        DeliveryModeType = DeliveryModeType.DayRelease,
                        DistanceInMiles = nearestLocationDayRelease.Distance,
                        Address1 = nearestLocationDayRelease.AddressLine1,
                        Address2 = nearestLocationDayRelease.Addressline2,
                        Town = nearestLocationDayRelease.Town,
                        County = nearestLocationDayRelease.County,
                        Postcode = nearestLocationDayRelease.Postcode
                    });

                var nearestLocationBlockRelease = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Provider && l.HasBlockReleaseDeliveryOption == true).MinBy(l => l.Distance);

                if (nearestLocationBlockRelease != null)
                    deliveryModels.Add(
                        new DeliveryModel
                        {
                            DeliveryModeType = DeliveryModeType.BlockRelease,
                            DistanceInMiles = nearestLocationBlockRelease.Distance,
                            Address1 = nearestLocationBlockRelease.AddressLine1,
                            Address2 = nearestLocationBlockRelease.Addressline2,
                            Town = nearestLocationBlockRelease.Town,
                            County = nearestLocationBlockRelease.County,
                            Postcode = nearestLocationBlockRelease.Postcode
                        });
            }

            return deliveryModels;
        }
    }
}