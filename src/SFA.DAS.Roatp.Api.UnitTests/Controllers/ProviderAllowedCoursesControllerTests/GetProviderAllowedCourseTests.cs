using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourse;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderAllowedCoursesControllerTests;

public class GetProviderAllowedCourseTests
{
    [Test, AutoData]
    public async Task WhenProviderAllowedCourseExists_ThenReturnsOk(GetProviderAllowedCourseQueryResult expectedResult)
    {
        // Arrange
        var ukprn = 12345678;
        var larsCode = "12345";
        Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock = new();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IUkprnAndLarsCodeValidator>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderAllowedCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var sut = new ProviderAllowedCoursesController(mediatorMock.Object, Mock.Of<ILogger<ProviderAllowedCoursesController>>(), validatorMock.Object);
        // Act
        var result = await sut.GetProviderAllowedCourse(ukprn, larsCode, CancellationToken.None);
        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(expectedResult, Is.EqualTo(okResult.Value));
    }

    [Test, AutoData]
    public async Task WhenProviderAllowedCourseDoesNotExist_ThenReturnsNoContent()
    {
        // Arrange
        var ukprn = 12345678;
        var larsCode = "12345";
        Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock = new();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IUkprnAndLarsCodeValidator>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderAllowedCourseQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProviderAllowedCourseQueryResult)null);
        var sut = new ProviderAllowedCoursesController(mediatorMock.Object, Mock.Of<ILogger<ProviderAllowedCoursesController>>(), validatorMock.Object);
        // Act
        var result = await sut.GetProviderAllowedCourse(ukprn, larsCode, CancellationToken.None);
        // Assert
        var noContentResult = result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
    }

    [Test, AutoData]
    public async Task WhenValidationFails_ThenReturnsNotFound()
    {
        // Arrange
        var ukprn = 12345678;
        var larsCode = "12345";
        Mock<IValidator<IUkprnAndLarsCodeValidator>> validatorMock = new();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IUkprnAndLarsCodeValidator>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Ukprn", "Ukprn is required")]));

        var sut = new ProviderAllowedCoursesController(Mock.Of<IMediator>(), Mock.Of<ILogger<ProviderAllowedCoursesController>>(), validatorMock.Object);
        // Act
        var result = await sut.GetProviderAllowedCourse(ukprn, larsCode, CancellationToken.None);
        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }
}
