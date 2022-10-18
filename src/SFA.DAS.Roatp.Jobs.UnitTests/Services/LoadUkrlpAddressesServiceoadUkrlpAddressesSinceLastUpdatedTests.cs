using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Requests;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class LoadUkrlpAddressesServiceoadUkrlpAddressesSinceLastUpdatedTests
    {
        [Test]
        [MoqAutoData]
        public async Task LoadUkrlpAddressesService_OnApiFailure_ReturnsFalse(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock)
        {
            var providers = new List<Provider>();
            var providerAddresses = new EditableList<UkrlpProviderAddress> { new() { Address1 = "1 Green Road" } };
            var request = new ProviderAddressLookupRequest();
            var _providersReadRepository = new Mock<IProvidersReadRepository>();
            _providersReadRepository.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);
            apiClientMock.Setup(a => a.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request)).ReturnsAsync((false, providerAddresses));

            var sut = new LoadUkrlpAddressesService(_providersReadRepository.Object, apiClientMock.Object, Mock.Of<IReloadProviderAddressesRepository>(), Mock.Of<IImportAuditWriteRepository>(), Mock.Of<IImportAuditReadRepository>(), Mock.Of<ILogger<LoadUkrlpAddressesService>>());

            var success = await sut.LoadUkrlpAddressesSinceLastUpdated();

            success.Should().BeFalse();
        }

        [Test]
        [MoqAutoData]
        public async Task LoadUkrlpAddressesService_OnNoResultFromApi_ReturnsFalse(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock)
        {
            var providers = new List<Provider>();
            var providerAddresses = new EditableList<UkrlpProviderAddress>();
            var request = new ProviderAddressLookupRequest();
            var _providersReadRepository = new Mock<IProvidersReadRepository>();
            _providersReadRepository.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);
            apiClientMock.Setup(a => a.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request)).ReturnsAsync((true, providerAddresses));

            var sut = new LoadUkrlpAddressesService(_providersReadRepository.Object, apiClientMock.Object, Mock.Of<IReloadProviderAddressesRepository>(), Mock.Of<IImportAuditWriteRepository>(), Mock.Of<IImportAuditReadRepository>(), Mock.Of<ILogger<LoadUkrlpAddressesService>>());

            var success = await sut.LoadUkrlpAddressesSinceLastUpdated();

            success.Should().BeFalse();
        }

        [Test]
        [MoqAutoData]
        public async Task LoadUkrlpAddressesService_OnAddressesReturned_ReturnsTrue(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IReloadProviderAddressesRepository> reloadProviderAddressesRepositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> importAuditWriteRepositoryMock,
            [Frozen] Mock<IImportAuditReadRepository> importAuditReadRepositoryMock
            )
        {
            var loggerMock = new Mock<ILogger<LoadUkrlpAddressesService>>();
            var ukprn = 1111111;
            var providerId = 5;
            var providers = new List<Provider> { new Provider { Ukprn = ukprn, Id = providerId } };
            var providerAddresses = new EditableList<UkrlpProviderAddress> { new() { Address1 = "1 Green Road", Ukprn = ukprn } };
        
            var _providersReadRepository = new Mock<IProvidersReadRepository>();
            _providersReadRepository.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);
            apiClientMock.Setup(a => a.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((true, providerAddresses));
        
            var sut = new LoadUkrlpAddressesService(_providersReadRepository.Object, apiClientMock.Object, reloadProviderAddressesRepositoryMock.Object, importAuditWriteRepositoryMock.Object, importAuditReadRepositoryMock.Object, loggerMock.Object);
        
            var success = await sut.LoadUkrlpAddressesSinceLastUpdated();
        
            success.Should().BeTrue();
            _providersReadRepository.Verify(x => x.GetAllProviders(), Times.Once);
            reloadProviderAddressesRepositoryMock.Verify(x => x.UpsertProviderAddresses(It.IsAny<List<ProviderAddress>>()), Times.Once);
            importAuditReadRepositoryMock.Verify(x => x.GetLastImportedDateByImportType(ImportType.ProviderAddresses), Times.Once);
            importAuditWriteRepositoryMock.Verify(x => x.Insert(It.IsAny<ImportAudit>()), Times.Once);
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
        
        // [Test]
        // [MoqAutoData]
        // public async Task LoadUkrlpAddressesService_OnAddressesReturnedAndNoProviderIdPresent_ReturnsTrue(
        //     [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
        //     [Frozen] Mock<IReloadProviderAddressesRepository> reloadProviderAddressesRepositoryMock,
        //     [Frozen] Mock<IImportAuditWriteRepository> importAuditWriteRepositoryMock
        // )
        // {
        //     var loggerMock = new Mock<ILogger<LoadUkrlpAddressesService>>();
        //     var ukprn = 1111111;
        //     var providerId = 5;
        //     var otherUkprn = 22222222;
        //     var providers = new List<Provider> { new Provider { Ukprn = otherUkprn, Id = providerId } };
        //     var providerAddresses = new EditableList<UkrlpProviderAddress> { new() { Address1 = "1 Green Road", Ukprn = ukprn } };
        //
        //     var _providersReadRepository = new Mock<IProvidersReadRepository>();
        //     _providersReadRepository.Setup(x => x.GetAllProviders()).ReturnsAsync(providers);
        //     apiClientMock.Setup(a => a.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", It.IsAny<ProviderAddressLookupRequest>())).ReturnsAsync((true, providerAddresses));
        //
        //     var sut = new LoadUkrlpAddressesService(_providersReadRepository.Object, apiClientMock.Object, reloadProviderAddressesRepositoryMock.Object, importAuditWriteRepositoryMock.Object, Mock.Of<IImportAuditReadRepository>(), loggerMock.Object);
        //
        //     var success = await sut.LoadUkrlpAddresses();
        //
        //     success.Should().BeTrue();
        //     _providersReadRepository.Verify(x => x.GetAllProviders(), Times.Once);
        //     reloadProviderAddressesRepositoryMock.Verify(x => x.ReloadProviderAddresses(It.IsAny<List<ProviderAddress>>()), Times.Once);
        //     importAuditWriteRepositoryMock.Verify(x => x.Insert(It.IsAny<ImportAudit>()), Times.Once);
        //     loggerMock.Verify(
        //         x => x.Log(
        //             It.Is<LogLevel>(l => l == LogLevel.Information),
        //             It.IsAny<EventId>(),
        //             It.IsAny<It.IsAnyType>(),
        //             It.IsAny<Exception>(),
        //             It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
        // }
    }
}
