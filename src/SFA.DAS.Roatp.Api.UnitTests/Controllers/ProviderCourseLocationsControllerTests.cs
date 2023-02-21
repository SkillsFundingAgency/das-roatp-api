using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

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
            List<ProviderCourseLocationModel> handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<List<ProviderCourseLocationModel>>(handlerResult));
            var result = await sut.GetProviderCourseLocations(ukprn, larsCode);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
