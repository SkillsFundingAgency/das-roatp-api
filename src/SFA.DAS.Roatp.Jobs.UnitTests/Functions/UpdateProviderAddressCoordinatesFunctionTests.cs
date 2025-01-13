using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

[TestFixture]
public class UpdateProviderAddressCoordinatesFunctionTests
{
    [Test]
    public async Task Run_ServiceReturnsTrue_LogInformation()
    {
        var loggerMock = new Mock<ILogger<UpdateProviderAddressCoordinatesFunction>>();
        var serviceMock = new Mock<IUpdateProviderAddressCoordinatesService>();
        serviceMock.Setup(x => x.UpdateProviderAddressCoordinates());
        var sut = new UpdateProviderAddressCoordinatesFunction(serviceMock.Object, loggerMock.Object);

        await sut.Run(default(TimerInfo));

        serviceMock.Verify(s => s.UpdateProviderAddressCoordinates());

        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(2));
    }
}
