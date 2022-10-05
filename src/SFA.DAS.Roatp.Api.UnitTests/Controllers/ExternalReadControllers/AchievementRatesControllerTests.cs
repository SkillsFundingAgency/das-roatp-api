using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Testing.AutoFixture;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers
{
    [TestFixture]
    public class AchievementRatesControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetOverallAchievementRates_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] AchievementRatesController sut,
            GetOverallAchievementRatesQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetOverallAchievementRatesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var response = await sut.GetOverallAchievementRates(It.IsAny<string>());

            var result = response as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var queryResult = result.Value as GetOverallAchievementRatesQueryResult;
            queryResult.OverallAchievementRates.Should().BeEquivalentTo(handlerResult.OverallAchievementRates);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetOverallAchievementRatesQuery>(), It.IsAny<CancellationToken>()));
        }
    }
}
