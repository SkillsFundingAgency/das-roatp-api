using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    [TestFixture]
    public class LoadProvidersUpdatedSinceLastTimeFunctionTests
    {
        [Test]
        public async Task Run_ServiceReturnsTrue_LogInformation()
        {
            var loggerMock = new Mock<ILogger<LoadProvidersAddressFunction>>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadProvidersAddresses()).ReturnsAsync(true);
            var sut = new LoadProvidersAddressFunction(serviceMock.Object, loggerMock.Object);

            await sut.Run(default(TimerInfo));

            serviceMock.Verify(s => s.LoadProvidersAddresses());
        }

        [Test]
        public async Task Run_ServiceReturnsFalse_LogWarning()
        {
            var loggerMock = new Mock<ILogger<LoadProvidersAddressFunction>>();
            var serviceMock = new Mock<ILoadUkrlpAddressesService>();
            serviceMock.Setup(x => x.LoadProvidersAddresses()).ReturnsAsync(false);
            var sut = new LoadProvidersAddressFunction(serviceMock.Object, loggerMock.Object);

            await sut.Run(default(TimerInfo));

            serviceMock.Verify(s => s.LoadProvidersAddresses());
        }
    }
}
