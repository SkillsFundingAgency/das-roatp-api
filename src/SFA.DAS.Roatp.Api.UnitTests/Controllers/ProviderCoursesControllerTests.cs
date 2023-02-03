using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCoursesControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetAllCourses_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCoursesController sut,
            int ukprn,
            GetAllProviderCoursesQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderCoursesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetAllProviderCoursesQueryResult>(handlerResult));
            var result = await sut.GetAllCourses(ukprn);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetCourse_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCoursesController sut,
            int ukprn,
            int larsCode,
            GetProviderCourseQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProviderCourseQueryResult>(handlerResult));
            var result = await sut.GetCourse(ukprn, larsCode);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
