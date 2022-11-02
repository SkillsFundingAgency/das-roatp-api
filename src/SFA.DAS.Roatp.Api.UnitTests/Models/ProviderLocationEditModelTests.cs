using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderLocationEditModelTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ReturnsCommand(ProviderLocationEditModel model)
        {
            ((UpdateProviderLocationDetailsCommand)model).Should().BeEquivalentTo(model);
        }
    }
}
