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
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderAllowedCoursesControllerTests;

public class DeleteProviderAllowedCoursesTests
{
    [Test, MoqAutoData]
    public async Task WhenUkprnAndLarsCodeAreValid_ReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<IUkprnAndLarsCodeValidator>(x => x.Ukprn == ukprn && x.LarsCode == larsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await sut.DeleteProviderAllowedCourse(ukprn, larsCode, userId, userDisplayName);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test, MoqAutoData]
    public async Task WhenUkprnAndLarsCodeAreInvalid_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        string larsCode,
        string userId,
        string userDisplayName)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<IUkprnAndLarsCodeValidator>(x => x.Ukprn == ukprn && x.LarsCode == larsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
            [
                new ValidationFailure(
                    nameof(IUkprnAndLarsCodeValidator.LarsCode),
                    ProviderCourseValidator.InvalidLarsCodeErrorMessage)
            ]));

        // Act
        var result = await sut.DeleteProviderAllowedCourse(ukprn, larsCode, userId, userDisplayName);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
