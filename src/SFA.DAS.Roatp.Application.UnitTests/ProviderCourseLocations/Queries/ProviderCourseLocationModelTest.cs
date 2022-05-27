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
    }
}