using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    internal class GetProviderControllerTests
    {
        private Mock<IGetProviderService> _getProviderServiceMock;
        private GetProviderController _sut;
        private const int _validUkprn = 10012002;

        [SetUp]
        public void Setup()
        {
            _getProviderServiceMock = new Mock<IGetProviderService>();
            _sut = new GetProviderController(Mock.Of<ILogger<GetProviderController>>(), _getProviderServiceMock.Object);
            _getProviderServiceMock.Setup(s => s.GetProvider(_validUkprn)).ReturnsAsync(new Provider() { Ukprn = _validUkprn });
        }

        [TestCase(0, typeof(BadRequestObjectResult))]
        [TestCase(-1, typeof(BadRequestObjectResult))]
        [TestCase(1, typeof(NotFoundObjectResult))]
        [TestCase(_validUkprn, typeof(OkObjectResult))]
        public async Task GetProvider_ReturnsAppropriateResponse(int ukprn, Type responseType)
        {
            var response = await _sut.GetProvider(ukprn);
            Assert.That(response.Result, Is.TypeOf(responseType));
        }
    }
}
