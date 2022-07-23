using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderLocationCreateModelTests
    {
        [Test, AutoData]
        public void Operator_ReturnsCommand(ProviderLocationCreateModel sut)
        {
            var command = (CreateProviderLocationCommand)sut;
            Assert.That(command, Is.Not.Null);
            command.Should().BeEquivalentTo(sut);
        }
    }
}
