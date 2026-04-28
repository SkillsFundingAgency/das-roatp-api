using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Azure.Core;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Requests;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services.ReloadProviderRegistrationDetailServiceTests;

[TestFixture]
public class ReloadAllAddressesTests
{
    [Test]
    [MoqAutoData]
    public async Task ReloadAllAddresses_OnApiError_ReturnsWithoutWritingToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = [new(), new()];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock
            .Setup(x => x.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address",
                It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((false, null));

        Func<Task> action = () => sut.ReloadAllAddresses();

        await action.Invoke();

        repositoryMock.Verify(x =>
                x.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()),
            Times.Never);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllAddresses_NoUkrlpResponse_ReturnsWithoutWritingToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = [new(), new()];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock
            .Setup(x => x.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address",
                It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((true, new()));

        Func<Task> action = () => sut.ReloadAllAddresses();

        await action.Invoke();

        repositoryMock.Verify(x =>
                x.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()),
            Times.Never);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllAddresses_UpdatesAddresses(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        List<UkrlpProviderAddress> ukrlpResponse,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = new List<ProviderRegistrationDetail>
        {
            new()
            {
                AddressLine1 = "OldAddress",
                AddressLine2 = "OldAddress2",
                AddressLine3 = "OldAddress3",
                AddressLine4 = "OldAddress4",
                Postcode = "OldPostcode",
                Town = "OldTown",
                Ukprn = 12345678
            }
        };
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        ukrlpResponse[0] = new UkrlpProviderAddress
        {
            Address1 = "TestAddress1",
            Address2 = "TestAddress2",
            Address3 = "TestAddress3",
            Address4 = "TestAddress4",
            Postcode = "TestPostcode",
            Town = "TestTown",
            Ukprn = 12345678
        };

        apiClientMock
            .Setup(x => x.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address",
                It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((true, ukrlpResponse));

        Func<Task> action = () => sut.ReloadAllAddresses();

        await action.Invoke();

        providerRegistrationDetails[0].AddressLine1.Should().BeEquivalentTo(ukrlpResponse[0].Address1);
        providerRegistrationDetails[0].AddressLine2.Should().BeEquivalentTo(ukrlpResponse[0].Address2);
        providerRegistrationDetails[0].AddressLine3.Should().BeEquivalentTo(ukrlpResponse[0].Address3);
        providerRegistrationDetails[0].AddressLine4.Should().BeEquivalentTo(ukrlpResponse[0].Address4);
        providerRegistrationDetails[0].Postcode.Should().BeEquivalentTo(ukrlpResponse[0].Postcode);
        providerRegistrationDetails[0].Town.Should().BeEquivalentTo(ukrlpResponse[0].Town);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllAddresses_WritesToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        List<UkrlpProviderAddress> ukrlpResponse,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = [new(), new()];
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock
            .Setup(x => x.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address",
                It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((true, ukrlpResponse));

        Func<Task> action = () => sut.ReloadAllAddresses();

        await action.Invoke();

        repositoryMock.Verify(x =>
                x.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()),
            Times.Once);
    }
}
