using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
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
            actualNationalLocation.LocationName.Should().BeNull();
            actualNationalLocation.IsImported.Should().BeFalse();
        }

        [Test, RecursiveMoqAutoData]
        public void CreateRegionalLocation_ReturnsNationalLocationInstance(Region region, int providerId)
        {
            var actualNationalLocation = ProviderLocation.CreateRegionalLocation(providerId, region);

            actualNationalLocation.LocationName.Should().BeNull();
            actualNationalLocation.NavigationId.Should().NotBeEmpty();
            actualNationalLocation.LocationType.Should().Be(LocationType.Regional);
            actualNationalLocation.Latitude.Should().Be(region.Latitude);
            actualNationalLocation.Longitude.Should().Be(region.Longitude);
            actualNationalLocation.IsImported.Should().BeFalse();
            actualNationalLocation.ProviderId = providerId;
        }
    }
}