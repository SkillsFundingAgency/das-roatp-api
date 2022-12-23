using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Models
{
    [TestFixture]
    public class PatchProviderTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(Provider source)
        {
            var destination = (PatchProvider)source;

            destination.MarketingInfo.Should().Be(source.MarketingInfo);
        }
    }
}
