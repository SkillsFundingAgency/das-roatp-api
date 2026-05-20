using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services.ReloadProviderRegistrationDetailServiceTests;

[TestFixture]
public class ReloadAllCoordinatesTests
{
    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_NoActiveProviders_DoesNotCallApi(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut)
    {
        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(new List<ProviderRegistrationDetail>());

        await sut.ReloadAllCoordinates();

        apiClientMock.Verify(x => x.Get<AddressList>(It.IsAny<string>()), Times.Never);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_NoPostcode_DoesNotUpdateCoordinates(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        double? latitude,
        double? longitude)
    {
        List<ProviderRegistrationDetail> providerRegistrationDetails = new List<ProviderRegistrationDetail>
        {
            new()
            {
                AddressLine1 = "OldAddress",
                AddressLine2 = "OldAddress2",
                AddressLine3 = "OldAddress3",
                AddressLine4 = "OldAddress4",
                Postcode = "",
                Town = "OldTown",
                Ukprn = 12345678,
                Longitude = longitude,
                Latitude = latitude
            }
        };

        AddressList addressList = new AddressList
        {
            Addresses = new List<LocationAddress>
            {
                new () { Longitude = 200, Latitude = 200 }
            }
        };

        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(a => a.Get<AddressList>($"lookup/addresses?postcode={providerRegistrationDetails[0].Postcode}")).ReturnsAsync((true, addressList));

        await sut.ReloadAllCoordinates();

        providerRegistrationDetails[0].Latitude.Should().Be(latitude);
        providerRegistrationDetails[0].Longitude.Should().Be(longitude);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_OnApiError_DoesNotUpdateCoordinates(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        double? latitude,
        double? longitude)
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
                Ukprn = 12345678,
                Longitude = longitude,
                Latitude = latitude
            }
        };

        AddressList addressList = new AddressList
        {
            Addresses = new List<LocationAddress>
            {
                new () { Longitude = 200, Latitude = 200 }
            }
        };

        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(a => a.Get<AddressList>($"lookup/addresses?postcode={providerRegistrationDetails[0].Postcode}")).ReturnsAsync((false, addressList));

        await sut.ReloadAllCoordinates();

        providerRegistrationDetails[0].Latitude.Should().Be(latitude);
        providerRegistrationDetails[0].Longitude.Should().Be(longitude);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_ApiResponseContainsNoLocations_DoesNotUpdateCoordinates(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        double? latitude,
        double? longitude)
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
                Ukprn = 12345678,
                Longitude = longitude,
                Latitude = latitude
            }
        };

        AddressList addressList = new AddressList
        {
            Addresses = new List<LocationAddress>()
        };

        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(a => a.Get<AddressList>($"lookup/addresses?postcode={providerRegistrationDetails[0].Postcode}")).ReturnsAsync((true, addressList));

        await sut.ReloadAllCoordinates();

        providerRegistrationDetails[0].Latitude.Should().Be(latitude);
        providerRegistrationDetails[0].Longitude.Should().Be(longitude);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_UpdatesCoordinates(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        double? latitude1,
        double? longitude1,
        double? latitude2,
        double? longitude2)
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
                Ukprn = 12345678,
                Longitude = longitude1,
                Latitude = latitude1
            }
        };

        AddressList addressList = new AddressList
        {
            Addresses = new List<LocationAddress>
            {
                new () { Longitude = longitude2, Latitude = latitude2 }
            }
        };

        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(a => a.Get<AddressList>($"lookup/addresses?postcode={providerRegistrationDetails[0].Postcode}")).ReturnsAsync((true, addressList));

        await sut.ReloadAllCoordinates();

        providerRegistrationDetails[0].Latitude.Should().Be(latitude2);
        providerRegistrationDetails[0].Longitude.Should().Be(longitude2);
    }

    [Test]
    [MoqAutoData]
    public async Task ReloadAllCoordinates_WritesToRepository(
        [Frozen] Mock<IProviderRegistrationDetailsWriteRepository> repositoryMock,
        [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        [Greedy] ReloadProviderRegistrationDetailService sut,
        double? latitude1,
        double? longitude1)
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
                Ukprn = 12345678,
                Longitude = longitude1,
                Latitude = latitude1
            }
        };

        AddressList addressList = new AddressList
        {
            Addresses = new List<LocationAddress>
            {
                new () { Longitude = 200, Latitude = 200 }
            }
        };

        repositoryMock.Setup(x => x.GetActiveProviders()).ReturnsAsync(providerRegistrationDetails);

        apiClientMock.Setup(a => a.Get<AddressList>($"lookup/addresses?postcode={providerRegistrationDetails[0].Postcode}")).ReturnsAsync((true, addressList));

        await sut.ReloadAllCoordinates();

        repositoryMock.Verify(x => x.UpdateProviders(
                It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()),
            Times.Once);
    }
}
