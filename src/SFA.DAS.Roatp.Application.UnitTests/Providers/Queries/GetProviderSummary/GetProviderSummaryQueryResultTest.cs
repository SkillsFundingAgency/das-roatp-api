using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderSummary
{
    [TestFixture]
    public class GetProviderSummaryQueryResultTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderRegistrationDetail source)
        {
            var model = (GetProviderSummaryQueryResult)source;

            model.Ukprn.Should().Be(source.Ukprn);
            model.Name.Should().Be(source.LegalName);
            model.ProviderTypeId.Should().Be(source.ProviderTypeId);
            model.StatusId.Should().Be(source.StatusId);
            model.TradingName.Should().Be(source.Provider.TradingName);
            model.Email.Should().Be(source.Provider.Email);
            model.Phone.Should().Be(source.Provider.Phone);
            model.ContactUrl.Should().Be(source.Provider.Website);
        }
    }
}
