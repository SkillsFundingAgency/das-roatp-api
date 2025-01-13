using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

[TestFixture]
public class ReloadProviderRegistrationDetailsFunctionTests
{
    [Test]
    public async Task Run_InvokesService()
    {
        var serviceMock = new Mock<IReloadProviderRegistrationDetailService>();
        var sut = new ReloadProviderRegistrationDetailsFunction(serviceMock.Object, Mock.Of<ILogger<ReloadProviderRegistrationDetailsFunction>>());

        await sut.Run(default);

        serviceMock.Verify(s => s.ReloadProviderRegistrationDetails());
    }
}
