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
    public class ReloadStandardsCacheFunctionTests
    {
        [Test]
        public async Task Run_CallsService()
        {
            var serviceMock = new Mock<IReloadStandardsCacheService>();
            var sut = new ReloadStandardsCacheFunction(serviceMock.Object);

            await sut.Run(default(TimerInfo), Mock.Of<ILogger>());

            serviceMock.Verify(s => s.ReloadStandardsCache());
        }
    }
}
