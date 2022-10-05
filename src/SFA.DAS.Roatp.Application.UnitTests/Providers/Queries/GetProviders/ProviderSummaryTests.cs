using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviders
{
    [TestFixture]
    public class ProviderSummaryTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(Provider source)
        {
            var model = (ProviderSummary)source;

            model.Ukprn.Should().Be(source.Ukprn);
            model.Name.Should().Be(source.LegalName);
            model.TradingName.Should().Be(source.TradingName);
            model.Email.Should().Be(source.Email);
            model.Phone.Should().Be(source.Phone);
            model.ContactUrl.Should().Be(source.Website);
            model.Address.Should().Be(source.Address);
        }

        [Test, RecursiveMoqAutoData]
        public void Operator_ReturnsNullModelFromNullEntity()
        {
            var model = (ProviderSummary)null;

            model.Should().BeNull();
        }
    }
}
