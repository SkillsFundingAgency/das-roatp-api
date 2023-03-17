using AutoFixture.NUnit3;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using FluentAssertions;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderAddModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsToCommand(ProviderAddModel source)
        {
            CreateProviderCommand cmd = source;
            cmd.Should().BeEquivalentTo(source); 
        }
    }
}
