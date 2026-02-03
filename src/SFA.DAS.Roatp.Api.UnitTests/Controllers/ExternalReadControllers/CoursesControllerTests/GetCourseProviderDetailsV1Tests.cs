using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Api.Models.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests;

public sealed class GetCourseProviderDetailsV1Tests
{
    [Test, MoqAutoData]
    public async Task GetCourseProviderDetails_InvokesQueryHandler(
        GetCourseProviderDetailsRequest request,
        GetCourseProviderDetailsQueryResult queryResult,
        int larsCode,
        int ukprn,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut
    )
    {
        queryResult.LarsCode = larsCode.ToString();
        mediatorMock.Setup(m => m.Send(
            It.Is<GetCourseProviderDetailsQuery>(q =>
                q.LarsCode == larsCode.ToString() &&
                q.Ukprn == ukprn &&
                q.Location == request.Location &&
                q.Longitude == request.Longitude &&
                q.ShortlistUserId == request.ShortlistUserId
            ),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new ValidatedResponse<GetCourseProviderDetailsQueryResult>(queryResult));

        var result = await sut.GetCourseProviderDetails(larsCode, ukprn, request);

        mediatorMock.Verify(m =>
            m.Send(It.Is<GetCourseProviderDetailsQuery>(
                q =>
                    q.Ukprn == ukprn &&
                    q.LarsCode == larsCode.ToString()
                    && q.Latitude == request.Latitude
                    && q.Longitude == request.Longitude
                    && q.Location == request.Location
                    && q.ShortlistUserId == request.ShortlistUserId
            ), It.IsAny<CancellationToken>()), Times.Once);

        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var v1Model = ok!.Value as GetCourseProviderDetailsResultModel;
        v1Model.Should().NotBeNull();

        v1Model!.Should().BeEquivalentTo(queryResult, options => options
            .ExcludingMissingMembers()
            .Excluding(x => x.LarsCode));

        v1Model.LarsCode.Should().Be(larsCode);
    }

    [Test, MoqAutoData]
    public async Task GetCourseProviderDetails_NoCourseDetailsFound_ReturnsNotFound(
        GetCourseProviderDetailsRequest request,
        int larsCode,
        int ukprn,
        GetCourseProviderDetailsQueryResult queryResult,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut)
    {
        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ValidatedResponse<GetCourseProviderDetailsQueryResult>)null);

        var result = await sut.GetCourseProviderDetails(larsCode, ukprn, request);

        result.Should().BeOfType<NotFoundResult>();
        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Test, MoqAutoData]
    public async Task GetCourseProviderDetails_InvalidResponse_ReturnsBadRequest(
        GetCourseProviderDetailsRequest request,
        int larsCode,
        int ukprn,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut)
    {
        var errors = new List<ValidationFailure>
        {
            new ValidationFailure("Ukprn", "Invalid Ukprn"),
            new ValidationFailure("LarsCode", "Invalid LarsCode")
        };

        mediatorMock.Setup(x => x.Send(It.IsAny<GetCourseProviderDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetCourseProviderDetailsQueryResult>(errors));

        var result = await sut.GetCourseProviderDetails(larsCode, ukprn, request);

        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.Value.Should().NotBeNull();
    }
}