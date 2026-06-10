using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

[TestFixture]
public class RefreshProviderDetailsFromUkrlpFunctionTests
{
    [Test]
    public async Task Run_InvokesServiceToRefreshProviderDetailsChangedRecently()
    {
        var loggerMock = new Mock<ILogger<RefreshProviderDetailsFromUkrlpFunction>>();
        var serviceMock = new Mock<IRefreshProviderDetailsFromUkrlpService>();
        serviceMock.Setup(x => x.RefreshProviderDetailsFromUkrlp(true)).Returns(Task.CompletedTask);
        var sut = new RefreshProviderDetailsFromUkrlpFunction(serviceMock.Object, loggerMock.Object);

        await sut.Run(default);

        serviceMock.Verify(s => s.RefreshProviderDetailsFromUkrlp(true));
    }
}
