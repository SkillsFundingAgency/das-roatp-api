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

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ReloadStandardsControllerTests
    {
        private Mock<IMediator> _mediator;
        private StandardsController _controller;

        [SetUp]
        public void Before_each_test()
        {
            _mediator = new Mock<IMediator>();
            _controller = new StandardsController(_mediator.Object, Mock.Of<ILogger<StandardsController>>());
        }

        [TestCase(true, HttpStatusCode.OK)]
        [TestCase(false, HttpStatusCode.BadRequest)]
        public async Task ReloadStandardsData_Successful_ReturnsSuccessfulStatus(bool returnStatus, HttpStatusCode expectedStatus)
        {
            var standards = new List<Standard>();
            var request = new StandardsRequest { Standards = standards };
            _mediator.Setup(x => x.Send(It.IsAny<ReloadStandardsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(returnStatus);
            var resultFromController = await _controller.ReloadStandardsData(request);
          
            var result = resultFromController as StatusCodeResult;
            Assert.AreEqual((int)expectedStatus, result.StatusCode);
        }
    }
}
