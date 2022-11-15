using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using System;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.CreateLocation
{
    [TestFixture]
    public class CreateProviderLocationCommandTests
    {
        [Test, AutoData]
        public void Operator_ReturnsProviderLocationEntity(CreateProviderLocationCommand sut)
        {
            var entity = (ProviderLocation)sut;

            Assert.IsNotNull(entity);
            entity.Should().BeEquivalentTo(sut, options => 
            {
                options.Excluding(c => c.Ukprn);
                options.Excluding(c => c.UserId);
                options.Excluding(c => c.UserDisplayName);
                return options;
            });
            entity.NavigationId.Should().NotBe(Guid.Empty);
            entity.IsImported.Should().BeFalse();
            entity.LocationType.Should().Be(LocationType.Provider);
        }
    }
}
