using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Queries
{
    [TestFixture]
    public class ProviderCourseLocationModelTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderCourseLocation location)
        {
            var model = (ProviderCourseLocationModel)location;

            model.Should().BeEquivalentTo(location, c => c
                 .Excluding(s => s.Id)
                 .Excluding(s => s.NavigationId)
                 .Excluding(s => s.ProviderCourseId)
                 .Excluding(s => s.ProviderLocationId)
                 .Excluding(s => s.Course)
                 .Excluding(s => s.Location)
            );
        }


        [TestCase(LocationType.Provider, "location name", "subregionName", "location name")]
        [TestCase(LocationType.Regional, "location name", "subregionName", "subregionName")]
        [TestCase(LocationType.National, "location name", null, null)]

        public void Operator_PopulatesModelFromEntity_SetsLocationNameBasedOnType( LocationType locationType, string locationName, string subregionName, string expectedLocationName)
        {
            var location = new ProviderCourseLocation() { Location = new ProviderLocation
                {
                    LocationType = locationType,
                    LocationName = locationName,
                    Region = new Region {SubregionName = subregionName}
                }
            };

            var model = (ProviderCourseLocationModel)location;
            model.LocationName.Should().Be(expectedLocationName);

        }
    }
}