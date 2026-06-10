using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

[TestFixture]
public class RefreshAllProviderDetailsFromUkrlpFunctionTests
{
    [Test]
    public async Task Run_InvokesServiceToRefreshAllProviderDetails()
    {
        var serviceMock = new Mock<IRefreshProviderDetailsFromUkrlpService>();
        serviceMock.Setup(x => x.RefreshProviderDetailsFromUkrlp(false)).Returns(Task.CompletedTask);
        var sut = new RefreshAllProviderDetailsFromUkrlpFunction(serviceMock.Object);

        await sut.Run(It.IsAny<HttpRequest>());

        serviceMock.Verify(s => s.RefreshProviderDetailsFromUkrlp(false));
    }
}
