using System;
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
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PutProviderAllowedCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderAllowedCoursesControllerTests;

public class PatchProviderAllowedCoursesTests
{
    [Test, MoqAutoData]
    public async Task PatchProviderAllowedCourse_UkprnAndLarsCodeAreValid_ThenUpdatesLastDateStarts_AndReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        PutProviderAllowedCourseModel request,
        int ukprn,
        string larsCode,
        DateTime lastDateStarts)
    {
        request.LastDateStarts = lastDateStarts;
        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<PutProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.PutProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task PatchProviderAllowedCourse_UkprnAndLarsCodeCombinationIsInvalid_ReturnsNotFound(
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        PutProviderAllowedCourseModel request,
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
        var result = await sut.PutProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task PutProviderAllowedCourse_CommandValidationFails_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        PutProviderAllowedCourseModel request,
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
                It.IsAny<PutProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.PutProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
