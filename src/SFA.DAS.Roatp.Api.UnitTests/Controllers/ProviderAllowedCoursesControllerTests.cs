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
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.AddProviderToAllowedCourse;
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
    public async Task AddProviderAllowedCourse_ReturnsCreatedResult(
        AddProviderToAllowedCourseCommand command,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut)
    {
        // Arrange
        mediatorMock
        .Setup(x => x.Send(
            It.Is<AddProviderToAllowedCourseCommand>(c =>
                c.Ukprn == command.Ukprn &&
                c.LarsCode == command.LarsCode &&
                c.UserId == command.UserId &&
                c.UserDisplayName == command.UserDisplayName &&
                c.LastDateStarts == command.LastDateStarts),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.AddProviderAllowedCourse(command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, command.LastDateStarts);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
    }

    [Test, MoqAutoData]
    public async Task AddProviderAllowedCourse_ResponseIsInvalid_ReturnsBadRequest(
        AddProviderToAllowedCourseCommand command,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderAllowedCoursesController sut)
    {
        // Arrange
        List<ValidationFailure> errors = new List<ValidationFailure>
        {
            new() { ErrorMessage = LarsCodeValidator.NotFoundMessage }
        };

        ValidatedResponse<Unit> validatedResponse = new(errors);

        mediatorMock
       .Setup(x => x.Send(
           It.Is<AddProviderToAllowedCourseCommand>(c =>
               c.Ukprn == command.Ukprn &&
               c.LarsCode == command.LarsCode &&
               c.UserId == command.UserId &&
               c.UserDisplayName == command.UserDisplayName &&
               c.LastDateStarts == command.LastDateStarts),
           It.IsAny<CancellationToken>()))
       .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.AddProviderAllowedCourse(command.Ukprn, command.LarsCode, command.UserId, command.UserDisplayName, command.LastDateStarts);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
