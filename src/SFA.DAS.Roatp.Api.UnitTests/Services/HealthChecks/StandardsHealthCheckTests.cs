using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.HealthCheck;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Services;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.HealthChecks
{
    [TestFixture]
    public class StandardsHealthCheckTests
    {
        private Mock<IGetStandardsService> _getStandardsService;
        private StandardsHealthCheck _standardsHealthCheck;

        [SetUp]
        public void Before_each_test()
        {
            _getStandardsService = new Mock<IGetStandardsService>();
            _standardsHealthCheck = new StandardsHealthCheck(_getStandardsService.Object);
        }

        [Test]
        public async Task StandardsHealthCheck_StandardsPresent_ReturnsHealthy()
        {
            var standards = new List<Standard>
            {
                new Standard { StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4", Title = "5", Version = "6" }
            };

            _getStandardsService.Setup(x => x.GetStandards()).ReturnsAsync(standards);

            var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
            _getStandardsService.Verify(x => x.GetStandards(), Times.Once);
        }

        [Test]
        public async Task StandardsHealthCheck_StandardsAbsent_ReturnsUnhealthy()
        {
            _getStandardsService.Setup(x => x.GetStandards()).ReturnsAsync(new List<Standard>());

            var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
            _getStandardsService.Verify(x => x.GetStandards(), Times.Once);
        }
    }
}
