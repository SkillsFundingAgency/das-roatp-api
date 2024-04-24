using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
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
        var sut = new ReloadProviderRegistrationDetailsFunction(serviceMock.Object);

        await sut.Run(default(TimerInfo));

        serviceMock.Verify(s => s.ReloadProviderRegistrationDetails());
    }
}
