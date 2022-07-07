using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class ProviderLocationModelTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderLocation location)
        {
            var model = (ProviderLocationModel)location;

            model.Should().BeEquivalentTo(location, c => c
                 .Excluding(s => s.Id)
                 .Excluding(s => s.ImportedLocationId)
                 .Excluding(s => s.ProviderId)
                 .Excluding(s => s.RegionId)
                 .Excluding(s => s.Provider)
                 .Excluding( s=>s.ProviderCourseLocations)
                 .Excluding(s => s.Region)
            );
        }

        [TestCase(LocationType.Provider, "location name", "address line 1", "location name")]
        [TestCase(LocationType.Regional, "location name", "address line 1", "address line 1")]
        [TestCase(LocationType.National, "location name", null, null)]

        public void Operator_PopulatesModelFromEntity_SetsLocationNameBasedOnType(LocationType locationType, string locationName, string addressLine1, string expectedLocationName)
        {
            var location = new ProviderLocation
            {
                LocationType = locationType,
                LocationName = locationName,
                AddressLine1 = addressLine1
            };

            var model = (ProviderLocationModel)location;
            model.LocationName.Should().Be(expectedLocationName);

        }
    }
}