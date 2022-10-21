using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class UpdateProviderAddressCoordinatesServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task UpdateProviderAddressCoordinatesService_OnSuccess(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IProviderAddressWriteRepository> providersAddressWriteRepository
            )
        {
            var latitude = 1;
            var longitude = 2;
            var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesService>>();
            var providerAddresses = new List<ProviderAddress> { new() { AddressLine1 = "1 Green Road",Postcode="XYZ"} };
            var addressList = new AddressList
            {
                Addresses = new List<LocationAddress>
                {
                    new()
                    {
                        Latitude = latitude, 
                        Longitude = longitude
                    }
                }
            };
            
            var providersAddressReadRepository = new Mock<IProviderAddressReadRepository>();
            providersAddressReadRepository.Setup(x => x.GetAllProviderAddresses()).ReturnsAsync(providerAddresses);

            apiClientMock.Setup(a => a.Get<AddressList>(It.IsAny<string>())).ReturnsAsync((true, addressList));

            var sut = new UpdateProviderAddressCoordinatesService(loggerMock.Object, apiClientMock.Object, providersAddressReadRepository.Object,providersAddressWriteRepository.Object);

            await sut.UpdateProviderAddressCoordinates();
            providersAddressReadRepository.Verify(x=>x.GetAllProviderAddresses(),Times.Once);
            providersAddressWriteRepository.Verify(x=>x.Update(It.IsAny<ProviderAddress>()),Times.Once);
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
        }

        [Test]
        [MoqAutoData]
        public async Task UpdateProviderAddressCoordinatesService_WhenNoPostcodePresent_NoUpdate(
           [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
           [Frozen] Mock<IProviderAddressWriteRepository> providersAddressWriteRepository
           )
        {
            var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesService>>();
            var providerAddresses = new List<ProviderAddress> { new() { AddressLine1 = "1 Green Road" } };
            

            var providersAddressReadRepository = new Mock<IProviderAddressReadRepository>();
            providersAddressReadRepository.Setup(x => x.GetAllProviderAddresses()).ReturnsAsync(providerAddresses);

            var sut = new UpdateProviderAddressCoordinatesService(loggerMock.Object, apiClientMock.Object, providersAddressReadRepository.Object, providersAddressWriteRepository.Object);

            await sut.UpdateProviderAddressCoordinates();
            providersAddressReadRepository.Verify(x => x.GetAllProviderAddresses(), Times.Once);
            providersAddressWriteRepository.Verify(x => x.Update(It.IsAny<ProviderAddress>()), Times.Never);
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);
        }

        [Test]
        [MoqAutoData]
        public async Task UpdateProviderAddressCoordinatesService_OnApiFailure(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IProviderAddressWriteRepository> providersAddressWriteRepository
        )
        {
            var latitude = 1;
            var longitude = 2;
            var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesService>>();
            var providerAddresses = new List<ProviderAddress>
                { new() { AddressLine1 = "1 Green Road", Postcode = "XYZ" } };
            var addressList = new AddressList
            {
                Addresses = new List<LocationAddress>
                {
                    new()
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    }
                }
            };

            var providersAddressReadRepository = new Mock<IProviderAddressReadRepository>();
            providersAddressReadRepository.Setup(x => x.GetAllProviderAddresses()).ReturnsAsync(providerAddresses);

            apiClientMock.Setup(a => a.Get<AddressList>(It.IsAny<string>())).ReturnsAsync((false, addressList));

            var sut = new UpdateProviderAddressCoordinatesService(loggerMock.Object, apiClientMock.Object,
                providersAddressReadRepository.Object, providersAddressWriteRepository.Object);

            await sut.UpdateProviderAddressCoordinates();
            providersAddressReadRepository.Verify(x => x.GetAllProviderAddresses(), Times.Once);
            
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task UpdateProviderAddressCoordinatesService_OnNoMatchingAddressesForPostcode(
              [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
              [Frozen] Mock<IProviderAddressWriteRepository> providersAddressWriteRepository
              )
        {
            var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesService>>();
            var providerAddresses = new List<ProviderAddress> { new() { AddressLine1 = "1 Green Road", Postcode = "XYZ" } };
            var addressList = new AddressList
            {
                Addresses = new List<LocationAddress>()
            };

            var providersAddressReadRepository = new Mock<IProviderAddressReadRepository>();
            providersAddressReadRepository.Setup(x => x.GetAllProviderAddresses()).ReturnsAsync(providerAddresses);

            apiClientMock.Setup(a => a.Get<AddressList>(It.IsAny<string>())).ReturnsAsync((true, addressList));

            var sut = new UpdateProviderAddressCoordinatesService(loggerMock.Object, apiClientMock.Object, providersAddressReadRepository.Object, providersAddressWriteRepository.Object);

            await sut.UpdateProviderAddressCoordinates();
            providersAddressReadRepository.Verify(x => x.GetAllProviderAddresses(), Times.Once);
            providersAddressWriteRepository.Verify(x => x.Update(It.IsAny<ProviderAddress>()), Times.Never);
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task UpdateProviderAddressCoordinatesService_OnFailureOfProviderAddressWrite(
             [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock
             )
        {
            var providersAddressWriteRepository = new Mock<IProviderAddressWriteRepository>();
            var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesService>>();
            var providerAddresses = new List<ProviderAddress> { new() { AddressLine1 = "1 Green Road", Postcode = "XYZ" } };
            var addressList = new AddressList
            {
                Addresses = new List<LocationAddress>()
            };

            var providersAddressReadRepository = new Mock<IProviderAddressReadRepository>();
            providersAddressReadRepository.Setup(x => x.GetAllProviderAddresses()).ReturnsAsync(providerAddresses);
            providersAddressWriteRepository.Setup(x => x.Update(It.IsAny<ProviderAddress>())).ReturnsAsync(false);
            apiClientMock.Setup(a => a.Get<AddressList>(It.IsAny<string>())).ReturnsAsync((true, addressList));

            var sut = new UpdateProviderAddressCoordinatesService(loggerMock.Object, apiClientMock.Object, providersAddressReadRepository.Object, providersAddressWriteRepository.Object);

            await sut.UpdateProviderAddressCoordinates();
            providersAddressReadRepository.Verify(x => x.GetAllProviderAddresses(), Times.Once);
            providersAddressWriteRepository.Verify(x => x.Update(It.IsAny<ProviderAddress>()), Times.Never);
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
