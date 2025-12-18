using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests;

public sealed class GetCourseProviderDetailsTests
{
    [Test]
    [MoqAutoData]
    public async Task GetCourseProviderDetails_InvokesQueryHandler(
        GetCourseProviderDetailsRequest request,
        GetCourseProviderDetailsQueryResult queryResult,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut
    )
    {
        var larsCode = "1";
        int ukprn = 2;

        mediatorMock.Setup(m => m.Send(
            It.Is<GetCourseProviderDetailsQuery>(q =>
                q.LarsCode == larsCode &&
                q.Ukprn == ukprn &&
                q.Location == request.Location &&
                q.Longitude == request.Longitude &&
                q.ShortlistUserId == request.ShortlistUserId
            ),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new ValidatedResponse<GetCourseProviderDetailsQueryResult>(queryResult));

        await sut.GetCourseProviderDetails(larsCode, ukprn, request);

        mediatorMock.Verify(m =>
            m.Send(It.Is<GetCourseProviderDetailsQuery>(
                q =>
                    q.Ukprn == ukprn &&
                    q.LarsCode == larsCode
                    && q.Latitude == request.Latitude
                    && q.Longitude == request.Longitude
                    && q.Location == request.Location
                    && q.ShortlistUserId == request.ShortlistUserId
            ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    [MoqAutoData]
    public async Task GetCourseProviderDetails_NoCourseDetailsFound_ReturnsNotFound(
        GetCourseProviderDetailsRequest request,
        GetCourseProviderDetailsQueryResult queryResult,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut)
    {
        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValidatedResponse<GetCourseProviderDetailsQueryResult>)null);

        var result = await sut.GetCourseProviderDetails("1", 1, request);

        result.Should().BeOfType<NotFoundResult>();
        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }
}