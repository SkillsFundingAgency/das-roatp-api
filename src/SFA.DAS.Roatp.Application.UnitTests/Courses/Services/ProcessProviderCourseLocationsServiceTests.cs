using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Services;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.Services
{
    [TestFixture]
    public class ProcessProviderCourseLocationsServiceTests
    {
        [Test]
        public void Service_NoProviderCourseLocations_ReturnsNotFoundRecord()
        {
            var providerLocations = new List<ProviderCourseLocationDetailsModel>();
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel> { new() { DeliveryModeType = DeliveryModeType.NotFound } };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Service_NullProviderCourseLocations_ReturnsNotFoundRecord()
        {
            List<ProviderCourseLocationDetailsModel> providerLocations = null;
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel> { new() { DeliveryModeType = DeliveryModeType.NotFound } };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Service_NationalProviderCourseLocations_ReturnsNationalRecord()
        {
            var providerLocations = new List<ProviderCourseLocationDetailsModel>
            {
                new()
                {
                    LocationType = LocationType.National
                }
            };
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel> { new() { DeliveryModeType = DeliveryModeType.National } };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Service_RegionalProviderCourseLocations_ReturnsRegionalRecordWithShortestDistance()
        {
            const int shortestDistance = 100;
            const int longestDistance = 150;
            var providerLocations = new List<ProviderCourseLocationDetailsModel>
            {
                new()
                {
                    LocationType = LocationType.Regional,
                    Distance = longestDistance
                },
                new()
                {
                    LocationType = LocationType.Regional,
                    Distance = shortestDistance
                }

            };
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel> { new() { DeliveryModeType = DeliveryModeType.Workplace, DistanceInMiles = shortestDistance } };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Service_DayReleaseProviderCourseLocations_ReturnsProviderRecordWithShortestDistance()
        {
            const int oneMile = 1;
            const int shortestDistance = 100;
            const int longestDistance = 150;
            const string address1 = "address 1";
            const string address2 = "address 2";
            const string town = "Town a";
            const string county = "County A";
            const string postcode = "xxx";
            var providerLocations = new List<ProviderCourseLocationDetailsModel>
            {
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = longestDistance,
                    HasDayReleaseDeliveryOption = true
                },
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = shortestDistance,
                    HasDayReleaseDeliveryOption = true,
                    AddressLine1 = address1,
                    Addressline2 = address2,
                    Town = town,
                    County = county,
                    Postcode = postcode
                },
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = oneMile,
                    HasDayReleaseDeliveryOption = false,
                    HasBlockReleaseDeliveryOption = true
                }
            };
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel>
            {
                new() {DeliveryModeType = DeliveryModeType.DayRelease, DistanceInMiles = shortestDistance, Address1 = address1,Address2 = address2, Town = town, County = county, Postcode = postcode},
                new() {DeliveryModeType = DeliveryModeType.BlockRelease, DistanceInMiles = oneMile}
            };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }


        [Test]
        public void Service_BlockReleaseProviderCourseLocations_ReturnsProviderRecordWithShortestDistance()
        {
            const int oneMile = 1;
            const int shortestDistance = 100;
            const int longestDistance = 150;
            const string address1 = "address 1";
            const string address2 = "address 2";
            const string town = "Town a";
            const string county = "County A";
            const string postcode = "xxx";
            var providerLocations = new List<ProviderCourseLocationDetailsModel>
            {
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = longestDistance,
                    HasBlockReleaseDeliveryOption = true
                },
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = shortestDistance,
                    HasBlockReleaseDeliveryOption = true,
                    AddressLine1 = address1,
                    Addressline2 = address2,
                    Town = town,
                    County = county,
                    Postcode = postcode
                },
                new()
                {
                    LocationType = LocationType.Provider,
                    Distance = oneMile,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = false
                }
            };
            var service = new ProcessProviderCourseLocationsService();
            var expectedResult = service.ConvertProviderLocationsToDeliveryModels(providerLocations);

            var actualResult = new List<DeliveryModel>
            {
                new() {DeliveryModeType = DeliveryModeType.BlockRelease, DistanceInMiles = shortestDistance, Address1 = address1,Address2 = address2, Town = town, County = county, Postcode = postcode},
                new() {DeliveryModeType = DeliveryModeType.DayRelease, DistanceInMiles = oneMile}
            };
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
