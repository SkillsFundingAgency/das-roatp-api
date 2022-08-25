using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Regions.Queries;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class RegionModelTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(Domain.Entities.Region region)
        {
            var model = (RegionModel)region;

            model.Should().BeEquivalentTo(region, c => c
                 .Excluding(s => s.Id)
                 .Excluding(s => s.Locations)
            );
        }
    }
}