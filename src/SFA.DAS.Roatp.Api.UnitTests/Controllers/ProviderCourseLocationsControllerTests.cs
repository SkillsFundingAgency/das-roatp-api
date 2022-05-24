using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
   
    [TestFixture]
    public class ProviderCourseLocationsControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetLocations_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCourseLocationsController sut,
             int ukprn,
             int larsCode,
            ProviderCourseLocationsQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<ProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetProviderCourseLocations(ukprn, larsCode);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.ProviderCourseLocations);
        }
    }
}
