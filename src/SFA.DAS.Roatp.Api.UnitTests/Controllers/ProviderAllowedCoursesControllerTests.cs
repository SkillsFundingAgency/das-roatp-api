using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.UpsertProviderAllowedCourse;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

public class ProviderAllowedCoursesControllerTests
{
    [Test, MoqAutoData]
    public void GetAllowedCourses_ReturnsOkResult(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        CourseType courseType,
        GetProviderAllowedCoursesQueryResult expected,
        CancellationToken cancellationToken)
    {
        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderAllowedCoursesQuery>(q => q.Ukprn == ukprn && q.CourseType == courseType), cancellationToken))
            .ReturnsAsync(expected);

        var result = sut.GetAllowedCourses(ukprn, courseType, cancellationToken).GetAwaiter().GetResult();

        result.As<OkObjectResult>().Value.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public async Task UpsertProviderAllowedCourse_ReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        UpsertProviderAllowedCourseModel request,
        int ukprn,
        string larsCode)
    {
        // Arrange
        mediatorMock
            .Setup(x => x.Send(
                It.Is<UpsertProviderAllowedCourseCommand>(c =>
                    c.Ukprn == ukprn &&
                    c.LarsCode == larsCode &&
                    c.UserId == request.UserId &&
                    c.UserDisplayName == request.UserDisplayName &&
                    c.LastDateStarts == request.LastDateStarts),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.UpsertProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task UpsertProviderAllowedCourse_ResponseIsInvalid_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        UpsertProviderAllowedCourseModel request,
        int ukprn,
        string larsCode)
    {
        // Arrange
        List<ValidationFailure> errors = new()
    {
        new() { ErrorMessage = LarsCodeValidator.NotFoundMessage }
    };

        ValidatedResponse<Unit> validatedResponse = new(errors);

        mediatorMock
            .Setup(x => x.Send(
                It.Is<UpsertProviderAllowedCourseCommand>(c =>
                    c.Ukprn == ukprn &&
                    c.LarsCode == larsCode &&
                    c.UserId == request.UserId &&
                    c.UserDisplayName == request.UserDisplayName &&
                    c.LastDateStarts == request.LastDateStarts),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.UpsertProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
