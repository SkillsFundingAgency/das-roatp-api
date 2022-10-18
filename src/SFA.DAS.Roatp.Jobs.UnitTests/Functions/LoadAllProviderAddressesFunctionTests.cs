using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    [TestFixture]
    public class LoadAllProviderAddressesFunctionTests
    {
        [Test]
        public async Task Run_ServiceReturnsTrue_LogInformation()
        {
            var loggerMock = new Mock<ILogger>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadUkrlpAddresses()).ReturnsAsync(true);
            var sut = new LoadAllProviderAddressesFunction(serviceMock.Object);

            await sut.Run(It.IsAny<HttpRequest>(),loggerMock.Object);

            serviceMock.Verify(s => s.LoadUkrlpAddresses());

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Never);

        }

        [Test]
        public async Task Run_ServiceReturnsFalse_LogWarning()
        {
            var loggerMock = new Mock<ILogger>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadUkrlpAddresses()).ReturnsAsync(false);
            var sut = new LoadAllProviderAddressesFunction(serviceMock.Object);

            await sut.Run(It.IsAny<HttpRequest>(), loggerMock.Object);

            serviceMock.Verify(s => s.LoadUkrlpAddresses());

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),Times.Never);


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
