using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.RestrictedCoursesControllerTests;

public class RestrictedCoursesControllerPostTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenCreateRestrictedCourseCommandIsValid_ThenReturnsCreatedResult(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] RestrictedCoursesController sut)
    {
        // Arrange
        mediatorMock
            .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.AddRestrictedCourse(command);

        // Assert
        Assert.That(result, Is.InstanceOf<CreatedResult>());
    }

    [Test]
    [MoqAutoData]
    public async Task WhenCreateRestrictedCourseResponseIsInvalid_ThenReturnsBadRequest(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] RestrictedCoursesController sut)
    {
        // Arrange
        List<ValidationFailure> errors = new List<ValidationFailure>
        {
            new() { ErrorMessage = LarsCodeValidator.NotFoundMessage }
        };

        ValidatedResponse<Unit> validatedResponse = new(errors);

        mediatorMock
            .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.AddRestrictedCourse(command);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}