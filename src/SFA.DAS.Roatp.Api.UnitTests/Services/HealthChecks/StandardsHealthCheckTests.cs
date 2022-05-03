using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.HealthCheck;
using SFA.DAS.Roatp.Application.StandardsCount;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.HealthChecks
{
     [TestFixture]
     public class StandardsHealthCheckTests
     {
         private Mock<IMediator> _mediatr;
         private StandardsHealthCheck _standardsHealthCheck;
    
         [SetUp]
         public void Before_each_test()
         {
             _mediatr = new Mock<IMediator>();
             _standardsHealthCheck = new StandardsHealthCheck(_mediatr.Object);
         }
    
         [Test]
         public async Task StandardsHealthCheck_StandardsPresent_ReturnsHealthy()
         {
             var standardsCount = 1;
             _mediatr.Setup(x => x.Send(It.IsAny<StandardsCountRequest>(), new CancellationToken() )).ReturnsAsync(standardsCount);
             var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
             Assert.AreEqual(HealthStatus.Healthy, result.Status);
             _mediatr.Verify(x => x.Send(It.IsAny<StandardsCountRequest>(), It.IsAny<CancellationToken>()), Times.Once);
         }
    
         [Test]
         public async Task StandardsHealthCheck_StandardsAbsent_ReturnsUnhealthy()
         {
             var standardsCount = 0;
            _mediatr.Setup(x => x.Send(It.IsAny<StandardsCountRequest>(), new CancellationToken())).ReturnsAsync(standardsCount);
             var result = await _standardsHealthCheck.CheckHealthAsync(new HealthCheckContext());
             Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
             _mediatr.Verify(x => x.Send(It.IsAny<StandardsCountRequest>(), It.IsAny<CancellationToken>()), Times.Once);
         }
    }
}
