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
    }
}
