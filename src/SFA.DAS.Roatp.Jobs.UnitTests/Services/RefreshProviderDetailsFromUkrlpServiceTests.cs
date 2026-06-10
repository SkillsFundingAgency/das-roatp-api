using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

public class RefreshProviderDetailsFromUkrlpServiceTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenRefreshingProviderDetails_ForRecentChanges_ThenIncludesAllProviderUkprns(List<Provider> providers)
    {
        // Arrange
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync(providers);
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(true);
        // Assert
        importAuditReadRepositoryMock.Verify(r => r.GetLastImportedDateByImportType(ImportType.Providers), Times.Once);
        courseManagementOuterApiClientMock.Verify(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.Is<GetUkrlpProvidersRequest>(s => s.Ukprns.SequenceEqual(providers.Select(p => p.Ukprn)))), Times.Once);
    }

    [Test, AutoData]
    public async Task WhenRefreshingProviderDetails_ForRecentChanges_ThenIncludesUpdatedSinceDateInRequest(DateTime expectedUpdateSinceDate)
    {
        // Arrange
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        importAuditReadRepositoryMock.Setup(r => r.GetLastImportedDateByImportType(ImportType.Providers)).ReturnsAsync(expectedUpdateSinceDate);
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync([]);
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(true);
        // Assert
        importAuditReadRepositoryMock.Verify(r => r.GetLastImportedDateByImportType(ImportType.Providers), Times.Once);
        courseManagementOuterApiClientMock.Verify(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.Is<GetUkrlpProvidersRequest>(s => s.UpdatedSinceDate == expectedUpdateSinceDate)), Times.Once);
    }

    [Test, AutoData]
    public async Task WhenRefreshingProviderDetails_ForRecentChanges_WithNoPreviousAudit_ThenDoesNotIncludesUpdatedSinceDateInRequest()
    {
        // Arrange
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        importAuditReadRepositoryMock.Setup(r => r.GetLastImportedDateByImportType(ImportType.Providers)).ReturnsAsync(() => null);
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync([]);
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(true);
        // Assert
        importAuditReadRepositoryMock.Verify(r => r.GetLastImportedDateByImportType(ImportType.Providers), Times.Once);
        courseManagementOuterApiClientMock.Verify(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.Is<GetUkrlpProvidersRequest>(s => s.UpdatedSinceDate == null)), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenRefreshingProviderDetails_ForAllRecords_ThenIncludesAllProvidersUkprnAndExcludesUpdatedSinceDateInRequest(List<Provider> providers)
    {
        // Arrange
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync(providers);
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(false);
        // Assert
        importAuditReadRepositoryMock.Verify(r => r.GetLastImportedDateByImportType(ImportType.Providers), Times.Never);
        courseManagementOuterApiClientMock.Verify(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.Is<GetUkrlpProvidersRequest>(s => s.UpdatedSinceDate == null && s.Ukprns.SequenceEqual(providers.Select(p => p.Ukprn)))), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenRefreshingProviderDetails_AndResponseIsEmpty_ThenReturnsWithNoFurtherAction(Provider provider)
    {
        // Arrange
        var name = provider.LegalName;
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        courseManagementOuterApiClientMock.Setup(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((true, new GetUkrlpProvidersResponse { Providers = [] }));
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync([provider]);
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(false);
        // Assert
        Assert.That(provider.LegalName, Is.EqualTo(name), "there is no change expected");
        providersWriteRepositoryMock.Verify(r => r.UpdateProviders(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ImportType>()), Times.Never, "there should be no update calls to the repository");
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenRefreshingProviderDetails_ThenUpdatesProviderDetails(Provider actual, ProviderDetails expected, int ukprn)
    {
        // Arrange
        actual.Ukprn = ukprn;
        actual.ProviderAddress.AddressUpdateDate = DateTime.UtcNow.AddDays(-1);
        expected.Ukprn = ukprn;
        var importAuditReadRepositoryMock = new Mock<IImportAuditReadRepository>();
        var providersWriteRepositoryMock = new Mock<IProvidersWriteRepository>();
        providersWriteRepositoryMock.Setup(r => r.GetAllProviders()).ReturnsAsync([actual]);
        var courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();
        courseManagementOuterApiClientMock.Setup(c => c.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(It.IsAny<string>(), It.IsAny<GetUkrlpProvidersRequest>())).ReturnsAsync((true, new GetUkrlpProvidersResponse { Providers = [expected] }));
        var sut = new RefreshProviderDetailsFromUkrlpService(courseManagementOuterApiClientMock.Object, importAuditReadRepositoryMock.Object, providersWriteRepositoryMock.Object, Mock.Of<ILogger<RefreshProviderDetailsFromUkrlpService>>());
        // Act
        await sut.RefreshProviderDetailsFromUkrlp(false);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actual.LegalName, Is.EqualTo(expected.LegalName));
            Assert.That(actual.TradingName, Is.EqualTo(expected.TradingName));
            Assert.That(actual.Email, Is.EqualTo(expected.PrimaryContact.Email));
            Assert.That(actual.Phone, Is.EqualTo(expected.PrimaryContact.Telephone));
            Assert.That(actual.Website, Is.EqualTo(expected.PrimaryContact.Website));

            Assert.That(actual.ProviderAddress, Is.Not.Null);
            Assert.That(actual.ProviderAddress.AddressLine1, Is.EqualTo(expected.LegalAddress.Address1));
            Assert.That(actual.ProviderAddress.AddressLine2, Is.EqualTo(expected.LegalAddress.Address2));
            Assert.That(actual.ProviderAddress.AddressLine3, Is.EqualTo(expected.LegalAddress.Address3));
            Assert.That(actual.ProviderAddress.AddressLine4, Is.EqualTo(expected.LegalAddress.Address4));
            Assert.That(actual.ProviderAddress.Town, Is.EqualTo(expected.LegalAddress.Town));
            Assert.That(actual.ProviderAddress.Postcode, Is.EqualTo(expected.LegalAddress.Postcode));
            Assert.That(actual.ProviderAddress.Latitude, Is.Null);
            Assert.That(actual.ProviderAddress.Longitude, Is.Null);
            Assert.That(actual.ProviderAddress.AddressUpdateDate.GetValueOrDefault().Date, Is.EqualTo(DateTime.UtcNow.Date));
        });
    }
}
