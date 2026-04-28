using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services.ReloadProviderRegistrationDetailServiceTests;

[TestFixture]
public class ReloadProviderDetailsTests
{
    [Test]
    [MoqAutoData]
    public async Task ReloadProviderDetails_OnApiError_ThrowsInvalidOperationException(
        [Frozen] Mock<IReloadProvidersRepository> reloadProvidersRepositoryMock,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        apiClientMock.Setup(x =>
            x.Get<List<RegisteredProviderModel>>("lookup/registered-providers")).ReturnsAsync((false, null));

        Func<Task> action = () => sut.ReloadProviderDetails();

        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadProviderDetails_NoProvidersToReload_DoesNotUpdateProviderDetails(
        [Frozen] Mock<IReloadProvidersRepository> reloadProvidersRepositoryMock,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        string existingTradingName,
        string existingLegalName,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        var registeredProviders = new List<RegisteredProviderModel>
        {
            new () { Ukprn = 12345678, TradingName = "NonMatchingTradingName", LegalName = "NonMatchingLegalName"}
        };

        apiClientMock.Setup(x =>
            x.Get<List<RegisteredProviderModel>>("lookup/registered-providers")).ReturnsAsync((true, registeredProviders));

        var providers = new List<Provider> { new() { Ukprn = 11223344, TradingName = existingTradingName, LegalName = existingLegalName } };

        providersReadRepositoryMock.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);

        Func<Task> action = () => sut.ReloadProviderDetails();

        await sut.ReloadProviderDetails();

        providers[0].TradingName.Should().Be(existingTradingName);
        providers[0].LegalName.Should().Be(existingLegalName);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadProviderDetails_UpdatesProviderDetails(
        [Frozen] Mock<IReloadProvidersRepository> reloadProvidersRepositoryMock,
        [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        string existingTradingName,
        string existingLegalName,
        string newLegalName,
        string newTradingName,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        var registeredProviders = new List<RegisteredProviderModel>
        {
            new () { Ukprn = 12345678, TradingName = newTradingName, LegalName = newLegalName}
        };

        apiClientMock.Setup(x =>
            x.Get<List<RegisteredProviderModel>>("lookup/registered-providers")).ReturnsAsync((true, registeredProviders));

        var providers = new List<Provider> { new() { Ukprn = 12345678, TradingName = existingTradingName, LegalName = existingLegalName } };

        providersReadRepositoryMock.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);

        Func<Task> action = () => sut.ReloadProviderDetails();

        await sut.ReloadProviderDetails();

        providers[0].TradingName.Should().Be(newTradingName);
        providers[0].LegalName.Should().Be(newLegalName);
    }
}
