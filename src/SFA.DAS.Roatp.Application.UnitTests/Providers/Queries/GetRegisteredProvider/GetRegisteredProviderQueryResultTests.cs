using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetRegisteredProvider;
public class GetRegisteredProviderQueryResultTests
{
    [Test, RecursiveMoqAutoData]
    public void Operator_ConvertsFromEntity(ProviderRegistrationDetail source)
    {
        GetRegisteredProviderQueryResult sut = source;

        sut.Ukprn.Should().Be(source.Ukprn);
        sut.Name.Should().Be(source.LegalName);
        sut.TradingName.Should().Be(source.Provider.TradingName);
        sut.Email.Should().Be(source.Provider.Email);
        sut.Phone.Should().Be(source.Provider.Phone);
        sut.ContactUrl.Should().Be(source.Provider.Website);
        sut.ProviderTypeId.Should().Be(source.ProviderTypeId);
        sut.StatusId.Should().Be(source.StatusId);
        sut.Address.Should().NotBeNull();
    }

    [TestCase(ProviderType.Supporting, ProviderStatusType.Active, false)]
    [TestCase(ProviderType.Main, ProviderStatusType.Active, true)]
    [TestCase(ProviderType.Main, ProviderStatusType.OnBoarding, true)]
    [TestCase(ProviderType.Main, ProviderStatusType.ActiveNoStarts, true)]
    [TestCase(ProviderType.Main, ProviderStatusType.Removed, false)]
    [TestCase(ProviderType.Employer, ProviderStatusType.Active, true)]
    [TestCase(ProviderType.Employer, ProviderStatusType.OnBoarding, true)]
    [TestCase(ProviderType.Employer, ProviderStatusType.ActiveNoStarts, true)]
    [TestCase(ProviderType.Employer, ProviderStatusType.Removed, false)]
    public void CanAccessApprenticeshipService_ReturnsExpectedValue(ProviderType providerType, ProviderStatusType providerStatusType, bool expected)
    {
        ProviderRegistrationDetail source = new()
        {
            Ukprn = 12345678,
            LegalName = "Test Provider",
            ProviderTypeId = (int)providerType,
            StatusId = (int)providerStatusType,
            Provider = new Provider
            {
                TradingName = "Test Trading Name",
                Email = "prov@test.com",
                Phone = "0123456789",
                Website = "http://www.testprovider.com",
            },
        };

        GetRegisteredProviderQueryResult sut = source;

        sut.CanAccessApprenticeshipService.Should().Be(expected);
    }
}
