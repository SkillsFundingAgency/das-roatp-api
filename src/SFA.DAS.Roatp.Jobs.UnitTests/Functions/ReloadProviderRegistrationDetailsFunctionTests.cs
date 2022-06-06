using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    [TestFixture]
    public class ReloadProviderRegistrationDetailsFunctionTests
    {
        [Test]
        public async Task Run_InvokesService()
        {
            var serviceMock = new Mock<IReloadProviderRegistrationDetailService>();
            var sut = new ReloadProviderRegistrationDetailsFunction(serviceMock.Object);

            await sut.Run(default(TimerInfo), Mock.Of<ILogger>());

            serviceMock.Verify(s => s.ReloadProviderRegistrationDetails());
        }
    }
}
