using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries;
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
            ProviderAllCoursesQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<ProviderAllCoursesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetAllCourses(ukprn);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.Courses);
        }


        [Test, MoqAutoData]
        public async Task GetCourse_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCoursesController sut,
            int ukprn,
            int larsCode,
            ProviderCourseQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<ProviderCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetCourse(ukprn, larsCode);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.Course);
        }
    }
}
