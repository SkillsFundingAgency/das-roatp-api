using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using static SFA.DAS.Roatp.Domain.Constants;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class ProviderLocationTests
    {
        [Test]
        public void CreateNationalLocation_ReturnsNationalLocationInstance()
        {
            var providerId = 1;
            var expectedNationalLocation = new ProviderLocation
            {
                ProviderId = providerId,
                Latitude = NationalLatLong.NationalLatitude,
                Longitude = NationalLatLong.NationalLongitude,
                LocationType = LocationType.National
            };

            var actualNationalLocation = ProviderLocation.CreateNationalLocation(providerId);

            actualNationalLocation.Should().BeEquivalentTo(expectedNationalLocation, option => option.Excluding(c => c.NavigationId));
        }
    }
}