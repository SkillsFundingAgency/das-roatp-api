using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.ImportProvider;
using SFA.DAS.Roatp.Domain.ApiModels.Import;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{

    [TestFixture]
    public class ImportProviderControllerTests
    {
        private Mock<IMediator> _mediator;
        private ImportProviderController _controller;

        [SetUp]
        public void Before_each_test()
        {
            _mediator = new Mock<IMediator>();
            _controller = new ImportProviderController(_mediator.Object, Mock.Of<ILogger<ImportProviderController>>());
        }

        [TestCase(true, HttpStatusCode.OK)]
        [TestCase(false, HttpStatusCode.BadRequest)]
        public async Task ImportProvider_ReturnsExpectedStatus(bool returnStatus, HttpStatusCode expectedStatus)
        { 
            var request = new ImportProviderRequest() { CdProvider = new CdProvider {Ukprn=12345678}};
            _mediator.Setup(x => x.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(returnStatus);
            var resultFromController = await _controller.ImportProvider(request);

            var result = resultFromController as StatusCodeResult;
            Assert.AreEqual((int)expectedStatus, result.StatusCode);
        }
    }
}
