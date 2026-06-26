using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.RestrictedCourses.Commands.AddRestrictedCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

public class RestrictedCoursesControllerPostTests
{
    [Test]
    [MoqAutoData]
    public async Task WhenCreateRestrictedCourseCommandIsValid_ThenReturnsNoContent(
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
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    [MoqAutoData]
    public async Task WhenCreateRestrictedCourseResponseIsInvalid_ThenReturnsBadRequest(
        AddRestrictedCourseCommand command,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] RestrictedCoursesController sut)
    {
        // Arrange
        var response = new ValidatedResponse<Unit>(
            new[]
            {
                new FluentValidation.Results.ValidationFailure(
                    nameof(command.LarsCode),
                    "Validation error")
            });

        mediatorMock
            .Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await sut.AddRestrictedCourse(command);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}