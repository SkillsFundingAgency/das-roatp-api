using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
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
    public async Task ReloadProviderDetails_NoProvidersToReload_DoesNotUpdateProviderDetails(
        [Frozen] Mock<IProvidersWriteRepository> providersWriteRepositoryMock,
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> providerRegistrationDetailsReadRepository,
        string existingTradingName,
        string existingLegalName,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        var providerRegistrationDetail = new List<ProviderRegistrationDetail>
        {
            new () { Ukprn = 12345678, TradingName = "NonMatchingTradingName", LegalName = "NonMatchingLegalName"}
        };

        providerRegistrationDetailsReadRepository.Setup(x =>
            x.GetActiveProviderRegistrations(CancellationToken.None)).ReturnsAsync(providerRegistrationDetail);

        var providers = new List<Provider> { new() { Ukprn = 11223344, TradingName = existingTradingName, LegalName = existingLegalName } };

        providersWriteRepositoryMock.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);

        await sut.ReloadProviderDetails();

        providers[0].TradingName.Should().Be(existingTradingName);
        providers[0].LegalName.Should().Be(existingLegalName);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadProviderDetails_UpdatesProviderDetails(
        [Frozen] Mock<IProvidersWriteRepository> providersWriteRepositoryMock,
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> providerRegistrationDetailsReadRepository,
        string existingTradingName,
        string existingLegalName,
        string newLegalName,
        string newTradingName,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        var providerRegistrationDetail = new List<ProviderRegistrationDetail>
        {
            new () { Ukprn = 12345678, TradingName = newTradingName, LegalName = newLegalName}
        };

        providerRegistrationDetailsReadRepository.Setup(x =>
            x.GetActiveProviderRegistrations(CancellationToken.None)).ReturnsAsync(providerRegistrationDetail);

        var providers = new List<Provider> { new() { Ukprn = 12345678, TradingName = existingTradingName, LegalName = existingLegalName } };

        providersWriteRepositoryMock.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);

        await sut.ReloadProviderDetails();

        providers[0].TradingName.Should().Be(newTradingName);
        providers[0].LegalName.Should().Be(newLegalName);

        providersWriteRepositoryMock.Verify(x => x.UpdateProviders
            (It.IsAny<DateTime>(), providers.Count, ImportType.Providers), Times.Once);
    }
}
