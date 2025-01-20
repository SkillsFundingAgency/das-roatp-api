using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersForCourseTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetProvidersForCourse_InvokesQueryHandler(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var larsCode = 1;
            var courseTitle = "test name";
            mediatorMock.Setup(m => m.Send(It.Is<GetProvidersForCourseQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProvidersForCourseQueryResult>(new GetProvidersForCourseQueryResult { CourseTitle = courseTitle }));

            var result = await sut.GetProvidersForCourse(larsCode, null, null);

            result.As<OkObjectResult>().Value.As<GetProvidersForCourseQueryResult>().CourseTitle.Should().Be(courseTitle);
            mediatorMock.Verify(m => m.Send(It.Is<GetProvidersForCourseQuery>(q => q.LarsCode == larsCode && q.Latitude == null && q.Longitude == null), It.IsAny<CancellationToken>()));
        }
    }
}
