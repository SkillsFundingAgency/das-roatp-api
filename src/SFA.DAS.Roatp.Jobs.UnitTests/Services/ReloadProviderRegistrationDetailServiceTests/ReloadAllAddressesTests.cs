using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services.ReloadProviderRegistrationDetailServiceTests;

[TestFixture]
public class ReloadAllAddressesTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUkrlpData_IncludesAllUkprnsAndAvoidsLastUpdatedDateInRequest(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Frozen] Mock<IImportAuditReadRepository> importAuditReadRepositoryMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        List<ProviderRegistrationDetail> providerRegistrationDetails,
        DateTime lastUpdatedDate)
    {
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);
        importAuditReadRepositoryMock.Setup(x => x.GetLastImportedDateByImportType(ImportType.ProviderRegistrationAddresses)).ReturnsAsync(lastUpdatedDate);

        apiClientMock.Setup(x => x.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((false, null));

        await sut.ReloadAllAddresses();

        apiClientMock.Verify(x => x.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl,
            It.Is<GetUkrlpProvidersRequest>(r =>
                r.Ukprns.SequenceEqual(providerRegistrationDetails.Select(p => p.Ukprn)) &&
                r.UpdatedSinceDate == null)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ReloadAllAddresses_OnOuterApiError_ReturnsWithoutWritingToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = [new(), new()];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(x => x.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((false, null));

        await sut.ReloadAllAddresses();

        repositoryMock.Verify(x => x.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task ReloadAllAddresses_UpdatesAddresses(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        ProviderRegistrationDetail actualProvider,
        ProviderDetails expectedProvider)
    {
        actualProvider.Ukprn = 12345678;
        List<ProviderRegistrationDetail> providerRegistrationDetails = [actualProvider];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        expectedProvider.Ukprn = actualProvider.Ukprn;
        GetUkrlpProvidersResponse ukrlpResponse = new() { Providers = [expectedProvider] };
        apiClientMock.Setup(x => x.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((true, ukrlpResponse));

        await sut.ReloadAllAddresses();

        actualProvider.AddressLine1.Should().BeEquivalentTo(expectedProvider.LegalAddress.Address1);
        actualProvider.AddressLine2.Should().BeEquivalentTo(expectedProvider.LegalAddress.Address2);
        actualProvider.AddressLine3.Should().BeEquivalentTo(expectedProvider.LegalAddress.Address3);
        actualProvider.AddressLine4.Should().BeEquivalentTo(expectedProvider.LegalAddress.Address4);
        actualProvider.Postcode.Should().BeEquivalentTo(expectedProvider.LegalAddress.Postcode);
        actualProvider.Town.Should().BeEquivalentTo(expectedProvider.LegalAddress.Town);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllAddresses_WritesToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        GetUkrlpProvidersResponse ukrlpResponse,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = [new(), new()];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock
            .Setup(x => x.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl,
                It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((true, new GetUkrlpProvidersResponse()));

        await sut.ReloadAllAddresses();

        repositoryMock.Verify(x => x.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()), Times.Once);
    }
}
