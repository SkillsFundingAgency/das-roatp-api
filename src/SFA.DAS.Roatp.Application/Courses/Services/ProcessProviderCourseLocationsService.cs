using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Courses.Services
{
    public class ProcessProviderCourseLocationsService : IProcessProviderCourseLocationsService
    {
        public List<DeliveryModel> ConvertProviderLocationsToDeliveryModels(
            List<ProviderCourseLocationDetailsModel> providerCourseLocations)
        {
            var deliveryModels = new List<DeliveryModel>();

            if (providerCourseLocations == null || !providerCourseLocations.Any())
            {
                return deliveryModels;
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.National))
            {
                deliveryModels.Add(
                    new DeliveryModel
                    {
                        LocationType = LocationType.National
                    });
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.Regional))
            {
                var nearestRegionalLocation = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Regional).MinBy(x => x.Distance);

                deliveryModels.Add(
                    new DeliveryModel
                    {
                        LocationType = LocationType.Regional,
                        DistanceInMiles = nearestRegionalLocation.Distance
                    });
            }

            if (providerCourseLocations.Any(l => l.LocationType == LocationType.Provider))
            {
                var nearestLocationDayRelease = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Provider && l.HasDayReleaseDeliveryOption == true)
                    .MinBy(l => l.Distance);

                if (nearestLocationDayRelease != null)
                    deliveryModels.Add(
                        new DeliveryModel
                        {
                            LocationType = LocationType.Provider,
                            DayRelease = true,
                            DistanceInMiles = nearestLocationDayRelease.Distance,
                        });

                var nearestLocationBlockRelease = providerCourseLocations
                    .Where(l => l.LocationType == LocationType.Provider && l.HasBlockReleaseDeliveryOption == true)
                    .MinBy(l => l.Distance);

                if (nearestLocationBlockRelease != null)
                    deliveryModels.Add(
                        new DeliveryModel
                        {
                            LocationType = LocationType.Provider,
                            BlockRelease = true,
                            DistanceInMiles = nearestLocationBlockRelease.Distance,
                        });
            }

            return deliveryModels;
        }

        public List<DeliveryModelWithAddress> ConvertProviderLocationsToDeliveryModelWithAddress(
            List<ProviderCourseLocationDetailsModel> providerCourseLocations)
            {
                var deliveryModels = new List<DeliveryModelWithAddress>();

                if (providerCourseLocations == null || !providerCourseLocations.Any())
                {
                    return deliveryModels;
                }

                if (providerCourseLocations.Any(l => l.LocationType == LocationType.National))
                {
                    deliveryModels.Add(
                        new DeliveryModelWithAddress
                        {
                            LocationType = LocationType.National
                        });
                }

                if (providerCourseLocations.Any(l => l.LocationType == LocationType.Regional))
                {
                    var nearestRegionalLocation = providerCourseLocations
                        .Where(l => l.LocationType == LocationType.Regional).MinBy(x => x.Distance);

                    deliveryModels.Add(
                        new DeliveryModelWithAddress
                        {
                            LocationType = LocationType.Regional,
                            DistanceInMiles = nearestRegionalLocation.Distance
                        });
                }

                if (providerCourseLocations.Any(l => l.LocationType == LocationType.Provider))
                {
                    var nearestLocationDayRelease = providerCourseLocations
                        .Where(l => l.LocationType == LocationType.Provider && l.HasDayReleaseDeliveryOption == true)
                        .MinBy(l => l.Distance);

                    if (nearestLocationDayRelease != null)
                        deliveryModels.Add(
                            new DeliveryModelWithAddress
                            {
                                LocationType = LocationType.Provider,
                                DayRelease = true,
                                DistanceInMiles = nearestLocationDayRelease.Distance,
                                Address1 = nearestLocationDayRelease.AddressLine1,
                                Address2 = nearestLocationDayRelease.AddressLine2,
                                Town = nearestLocationDayRelease.Town,
                                County = nearestLocationDayRelease.County,
                                Postcode = nearestLocationDayRelease.Postcode
                            });

                    var nearestLocationBlockRelease = providerCourseLocations
                        .Where(l => l.LocationType == LocationType.Provider && l.HasBlockReleaseDeliveryOption == true)
                        .MinBy(l => l.Distance);

                    if (nearestLocationBlockRelease != null)
                        deliveryModels.Add(
                            new DeliveryModelWithAddress
                            {
                                LocationType = LocationType.Provider,
                                BlockRelease = true,
                                DistanceInMiles = nearestLocationBlockRelease.Distance,
                                Address1 = nearestLocationBlockRelease.AddressLine1,
                                Address2 = nearestLocationBlockRelease.AddressLine2,
                                Town = nearestLocationBlockRelease.Town,
                                County = nearestLocationBlockRelease.County,
                                Postcode = nearestLocationBlockRelease.Postcode
                            });
                }

                return deliveryModels;
            }
        }
    }
