using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Commands.PatchProviderAllowedCourse;
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
    public async Task UpsertProviderAllowedCourse_ValidationPasses_ReturnsNoContentResult(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
    [Greedy] ProviderAllowedCoursesController sut,
    UpsertProviderAllowedCourseModel request,
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
                It.IsAny<UpsertProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.UpsertProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task UpsertProviderAllowedCourse_ControllerValidationFails_ReturnsNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
    [Greedy] ProviderAllowedCoursesController sut,
    UpsertProviderAllowedCourseModel request,
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
        var result = await sut.UpsertProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task UpsertProviderAllowedCourse_ResponseIsInvalid_ReturnsBadRequest(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
    [Greedy] ProviderAllowedCoursesController sut,
    UpsertProviderAllowedCourseModel request,
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
        new ValidationFailure(nameof(IUkprnAndLarsCodeValidator.LarsCode), LarsCodeValidator.NotFoundMessage)
    };

        var validatedResponse = new ValidatedResponse<Unit>(errors);

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<UpsertProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.UpsertProviderAllowedCourse(ukprn, larsCode, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task UpdateLastDateStarts_ValidationPasses_ReturnsNoContentResult(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
    [Greedy] ProviderAllowedCoursesController sut,
    PatchProviderAllowedCourseModel request,
    int ukprn,
    string larsCode,
    string userId,
    string userDisplayName)
    {
        // Arrange
        var patchDoc = CreatePatchDoc(request);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<PatchProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.PatchProviderAllowedCourse(ukprn, larsCode, patchDoc, userId, userDisplayName);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task UpdateLastDateStarts_ControllerValidationFails_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        PatchProviderAllowedCourseModel request,
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName)
    {
        // Arrange
        var patchDoc = CreatePatchDoc(request);

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
        var result = await sut.PatchProviderAllowedCourse(ukprn, larsCode, patchDoc, userId, userDisplayName);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task UpdateLastDateStarts_ResponseIsInvalid_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        PatchProviderAllowedCourseModel request,
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName)
    {
        // Arrange
        var patchDoc = CreatePatchDoc(request);

        validatorMock
            .Setup(v => v.ValidateAsync(
                It.Is<IUkprnAndLarsCodeValidator>(x =>
                    x.Ukprn == ukprn &&
                    x.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var errors = new List<ValidationFailure>
    {
        new ValidationFailure(nameof(IUkprnAndLarsCodeValidator.LarsCode), LarsCodeValidator.NotFoundMessage)
    };

        var validatedResponse = new ValidatedResponse<Unit>(errors);

        mediatorMock
            .Setup(x => x.Send(
                It.IsAny<PatchProviderAllowedCourseCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.PatchProviderAllowedCourse(ukprn, larsCode, patchDoc, userId, userDisplayName);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    private static JsonPatchDocument<PatchProviderAllowedCourseModel> CreatePatchDoc(
        PatchProviderAllowedCourseModel request)
    {
        var patchDoc = new JsonPatchDocument<PatchProviderAllowedCourseModel>();

        patchDoc.Replace(x => x.LastDateStarts, request.LastDateStarts);

        return patchDoc;
    }
}
