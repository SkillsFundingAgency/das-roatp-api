using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Api.Requests;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class StandardsReloadControllerTests
    {
        private Mock<IMediator> _mediator;
        private StandardsReloadController _reloadController;

        [SetUp]
        public void Before_each_test()
        {
            _mediator = new Mock<IMediator>();
            _reloadController = new StandardsReloadController(_mediator.Object, Mock.Of<ILogger<StandardsReloadController>>());
        }

        [TestCase(true, HttpStatusCode.OK)]
        [TestCase(false, HttpStatusCode.BadRequest)]
        public async Task ReloadStandardsData_Successful_ReturnsSuccessfulStatus(bool returnStatus, HttpStatusCode expectedStatus)
        {
            var standards = new List<Standard>();
            var request = new StandardsRequest { Standards = standards };
            _mediator.Setup(x => x.Send(It.IsAny<ReloadStandardsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(returnStatus);
            var resultFromController = await _reloadController.ReloadStandardsData(request);
          
            var result = resultFromController as StatusCodeResult;
            Assert.AreEqual((int)expectedStatus, result.StatusCode);
        }
    }
}
