using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.CreateProvider
{
    [TestFixture]
    public class CreateProviderCommandTests
    {
        [Test, AutoData]
        public void Operator_TransformsCommandToEntity(CreateProviderCommand sut)
        {
            Domain.Entities.Provider provider = sut;
            provider.Should().BeEquivalentTo(sut, option => option.ExcludingMissingMembers());
        }
    }
}
