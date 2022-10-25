using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersCountForCourseTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetProvidersCountForCourse_InvokesQueryHandler(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var larsCode = 1;
            var expectedCount = 10;
            mediatorMock.Setup(m => m.Send(It.Is<GetProvidersCountForCourseQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(new GetProvidersCountForCourseQueryResult { ProvidersCount = expectedCount });

            var result = await sut.GetProvidersCountForCourse(larsCode);

            Assert.AreEqual(expectedCount, result.Result.As<OkObjectResult>().Value.As<GetProvidersCountForCourseQueryResult>().ProvidersCount);
            mediatorMock.Verify(m => m.Send(It.Is<GetProvidersCountForCourseQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()));
        }
    }
}
