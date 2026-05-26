using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    [TestFixture]
    public class LoadAllProviderAddressesFunctionTests
    {
        [Test]
        public async Task Run_ServiceReturnsTrue_LogInformation()
        {
            var loggerMock = new Mock<ILogger<LoadAllProviderAddressesFunction>>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadAllProvidersAddresses()).ReturnsAsync(true);
            var sut = new LoadAllProviderAddressesFunction(serviceMock.Object, loggerMock.Object);

            await sut.Run(It.IsAny<HttpRequest>());

            serviceMock.Verify(s => s.LoadAllProvidersAddresses());
        }

        [Test]
        public async Task Run_ServiceReturnsFalse_LogWarning()
        {
            var loggerMock = new Mock<ILogger<LoadAllProviderAddressesFunction>>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadAllProvidersAddresses()).ReturnsAsync(false);
            var sut = new LoadAllProviderAddressesFunction(serviceMock.Object, loggerMock.Object);

            await sut.Run(It.IsAny<HttpRequest>());

            serviceMock.Verify(s => s.LoadAllProvidersAddresses());
        }
    }
}
