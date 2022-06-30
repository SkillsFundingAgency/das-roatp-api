using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Region.Queries;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class RegionsControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetRegions_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] RegionsController sut,
            RegionsQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<RegionsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetRegions();

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.Regions);
        }
    }
}