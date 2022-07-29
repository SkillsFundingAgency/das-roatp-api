using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Standards.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class StandardsControllerTests
    {
        [Test, AutoData]
        public async Task GetAllStandards_ReturnsListOfStandards(List<Standard> standards)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(r => r.Send(It.IsAny<GetAllStandardsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetAllStandardsQueryResult(standards));
            var sut = new StandardsController(Mock.Of<ILogger<StandardsController>>(), mediatorMock.Object);

            var response = await sut.GetAllStandards();

            var result = response.Result as OkObjectResult;
            result.Should().NotBeNull();
            var queryResult = result.Value as GetAllStandardsQueryResult;
            queryResult.Standards.Should().BeEquivalentTo(standards);
        }
    }
}
