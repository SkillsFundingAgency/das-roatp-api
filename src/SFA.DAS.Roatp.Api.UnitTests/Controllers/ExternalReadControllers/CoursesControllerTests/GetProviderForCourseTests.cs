using AutoFixture.NUnit3;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProviderForCourseTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetProviderForCourse_InvokesQueryHandler(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var larsCode = 1;
            var ukprn = 11112222;
            var name = "test name";
            mediatorMock.Setup(m => m.Send(It.Is<GetProviderDetailsForCourseQuery>(q => q.LarsCode == larsCode && q.Ukprn==ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetProviderDetailsForCourseQueryResult{Name=name});

            var result = await sut.GetProviderForCourse(larsCode, ukprn, null, null);

            Assert.AreEqual(name, result.Result.As<OkObjectResult>().Value.As<GetProviderDetailsForCourseQueryResult>().Name);
            mediatorMock.Verify(m => m.Send(It.Is<GetProviderDetailsForCourseQuery>(q => q.LarsCode == larsCode && q.Ukprn==ukprn && q.Latitude==null && q.Longitude == null), It.IsAny<CancellationToken>()));
        }
    }
}
