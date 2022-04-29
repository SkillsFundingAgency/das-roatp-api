using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.HealthCheck;
using SFA.DAS.Roatp.Api.Services;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.HealthChecks
{
    [TestFixture]
    public class StandardsHealthCheckTests
    {
        private Mock<IGetStandardsCountService> _getStandardsService;
        private StandardsHealthCheck _standardsHealthCheck;

        [SetUp]
        public void Before_each_test()
        {
            _getStandardsService = new Mock<IGetStandardsCountService>();
            _standardsHealthCheck = new StandardsHealthCheck(_getStandardsService.Object);
        }

        [Test]
        public async Task StandardsHealthCheck_StandardsPresent_ReturnsHealthy()
        {

            _getStandardsService.Setup(x => x.GetStandardsCount()).ReturnsAsync(1);

            var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
            _getStandardsService.Verify(x => x.GetStandardsCount(), Times.Once);
        }

        [Test]
        public async Task StandardsHealthCheck_StandardsAbsent_ReturnsUnhealthy()
        {
            _getStandardsService.Setup(x => x.GetStandardsCount()).ReturnsAsync(0);

            var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
            _getStandardsService.Verify(x => x.GetStandardsCount(), Times.Once);
        }
    }
}
