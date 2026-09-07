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
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.RestrictProvider;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderCourseTypesControllerTests;

public class ProviderCourseTypesControllerPostTests
{
    [Test, MoqAutoData]
    public async Task WhenRestrictProviderCommandIsValid_ThenReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCourseTypesController sut)
    {
        // Arrange
        var ukprn = 12345678;
        var courseType = CourseType.Apprenticeship;

        var request = new RestrictProviderModel
        {
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        var command = new RestrictProviderCommand
        {
            Ukprn = ukprn,
            CourseType = courseType,
            UserId = request.UserId,
            UserDisplayName = request.UserDisplayName
        };

        mediatorMock
            .Setup(x => x.Send(
                It.Is<RestrictProviderCommand>(c =>
                    c.Ukprn == command.Ukprn &&
                    c.CourseType == command.CourseType &&
                    c.UserId == command.UserId &&
                    c.UserDisplayName == command.UserDisplayName),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.RestrictProvider(ukprn, courseType, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        mediatorMock.Verify(x => x.Send(
            It.Is<RestrictProviderCommand>(c =>
                c.Ukprn == command.Ukprn &&
                c.CourseType == command.CourseType &&
                c.UserId == command.UserId &&
                c.UserDisplayName == command.UserDisplayName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenRestrictProviderResponseIsInvalid_ThenReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCourseTypesController sut)
    {
        // Arrange
        var ukprn = 12345678;
        var courseType = CourseType.Apprenticeship;

        var request = new RestrictProviderModel
        {
            UserId = "TestUserId",
            UserDisplayName = "TestUser"
        };

        List<ValidationFailure> errors = new()
        {
            new()
            {
                ErrorMessage = RestrictProviderCommandValidator.ProviderNotFound
            }
        };

        ValidatedResponse<Unit> validatedResponse = new(errors);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<RestrictProviderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.RestrictProvider(ukprn, courseType, request);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test, MoqAutoData]
    public async Task WhenRequestIsValid_ThenReturnsNoContentResult(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprn>> validatorMock,
    [Greedy] ProviderCourseTypesController sut,
    AddCourseTypesModel request,
    int ukprn)
    {
        // Arrange
        request.CourseTypes = ["Apprenticeship"];

        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateProviderCourseTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.AddCourseTypes(ukprn, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task WhenInvalidUkprn_ThenReturnsNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprn>> validatorMock,
    [Greedy] ProviderCourseTypesController sut,
    AddCourseTypesModel request,
    int ukprn)
    {
        // Arrange
        request.CourseTypes = ["Apprenticeship"];

        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure(nameof(IUkprn.Ukprn), "Invalid UKPRN") }));

        // Act
        var result = await sut.AddCourseTypes(ukprn, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        mediatorMock
            .Verify(x => x.Send(It.IsAny<CreateProviderCourseTypeCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenValidationFails_ThenReturnsBadRequest(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<IUkprn>> validatorMock,
    [Greedy] ProviderCourseTypesController sut,
    AddCourseTypesModel request,
    int ukprn)
    {
        // Arrange
        request.CourseTypes = ["Apprenticeship"];

        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var errors = new List<ValidationFailure>
            {
                new(nameof(CreateProviderCourseTypeCommand.CourseTypes),CreateProviderCourseTypeCommandValidator.CourseTypesExist)
            };

        var validatedResponse = new ValidatedResponse<Unit>(errors);

        mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateProviderCourseTypeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.AddCourseTypes(ukprn, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
