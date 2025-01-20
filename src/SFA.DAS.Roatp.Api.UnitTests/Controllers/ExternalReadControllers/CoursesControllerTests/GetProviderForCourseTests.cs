using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    public class GetProviderForCourseTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetProviderDetailsForCourse_InvokesQueryHandler(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var larsCode = 1;
            var ukprn = 11112222;
            var name = "test name";
            mediatorMock.Setup(m => m.Send(It.Is<GetProviderDetailsForCourseQuery>(q => q.LarsCode == larsCode && q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProviderDetailsForCourseQueryResult>(new GetProviderDetailsForCourseQueryResult { Name = name }));

            var result = await sut.GetProviderDetailsForCourse(larsCode, ukprn, null, null);

            result.As<OkObjectResult>().Value.As<GetProviderDetailsForCourseQueryResult>().Name.Should().Be(name);
            mediatorMock.Verify(m => m.Send(It.Is<GetProviderDetailsForCourseQuery>(q => q.LarsCode == larsCode && q.Ukprn == ukprn && q.Latitude == null && q.Longitude == null), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void ControllerConvention_HasRequiredNamespace()
        {
            typeof(CoursesController).Namespace.EndsWith("ExternalReadControllers").Should().BeTrue();
        }

        [Test]
        [MoqAutoData]
        public async Task GetProviderDetailsForCourse_InvokesQueryHandler_NoResultGivesBadRequest(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var larsCode = 1;
            var ukprn = 11112222;
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderDetailsForCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProviderDetailsForCourseQueryResult>((GetProviderDetailsForCourseQueryResult)null));

            var response = await sut.GetProviderDetailsForCourse(larsCode, ukprn, null, null);

            mediatorMock.Verify(m => m.Send(It.Is<GetProviderDetailsForCourseQuery>(q => q.LarsCode == larsCode && q.Ukprn == ukprn && q.Latitude == null && q.Longitude == null), It.IsAny<CancellationToken>()));

            (((NotFoundResult)response)!).StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
