using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.CreateProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderAllowedCoursesControllerTests;

public class AddProviderAllowedCoursesTests
{
    [Test, MoqAutoData]
    public async Task AddProviderAllowedCourse_ValidationPasses_ReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        CreateProviderAllowedCourseModel request,
        int ukprn,
        string larsCode)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.AddProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task AddProviderAllowedCourse_ControllerValidationFails_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        CreateProviderAllowedCourseModel request,
        int ukprn,
        string larsCode)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure(
                    nameof(IUkprnAndLarsCodeValidator.LarsCode),
                    ProviderCourseValidator.InvalidLarsCodeErrorMessage)
            }));

        // Act
        var result = await sut.AddProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task AddProviderAllowedCourse_ResponseIsInvalid_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        CreateProviderAllowedCourseModel request,
        int ukprn,
        string larsCode)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var errors = new List<ValidationFailure>
        {
            new (nameof(IUkprnAndLarsCodeValidator.LarsCode), LarsCodeValidator.NotFoundMessage)
        };

        var validatedResponse = new ValidatedResponse<Unit>(errors);

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<CreateProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.AddProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
