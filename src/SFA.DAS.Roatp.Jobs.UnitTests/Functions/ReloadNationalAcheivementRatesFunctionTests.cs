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
    public class ReloadNationalAcheivementRatesFunctionTests
    {
        [Test]
        public async Task Run_InvokesService()
        {
            var serviceMock = new Mock<IReloadNationalAcheivementRatesLookupService>();
            var sut = new ReloadNationalAcheivementRatesFunction(serviceMock.Object);

            await sut.Run(default(TimerInfo), Mock.Of<ILogger>());

            serviceMock.Verify(s => s.ReloadNationalAcheivementRates());
        }
    }
}
