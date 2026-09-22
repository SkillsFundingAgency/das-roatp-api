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
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderRestrictedCoursesControllerTests;

public class GetNotRestrictedApprenticeshipsTests
{
    [Test, MoqAutoData]
    public async Task WhenRequestIsValid_ThenReturnsOk(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprn>> validatorMock,
        [Greedy] ProviderRestrictedCoursesController sut,
        GetProviderNotRestrictedApprenticeshipsQueryResult queryResult,
        int ukprn)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(It.Is<GetProviderNotRestrictedApprenticeshipsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>(queryResult));

        // Act
        var result = await sut.GetNotRestrictedApprenticeships(ukprn);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task WhenUkprnIsInvalid_ThenReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprn>> validatorMock,
        [Greedy] ProviderRestrictedCoursesController sut,
        int ukprn)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure(nameof(IUkprn.Ukprn), "Invalid UKPRN") }));

        // Act
        var result = await sut.GetNotRestrictedApprenticeships(ukprn);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        mediatorMock
            .Verify(x => x.Send(It.IsAny<GetProviderNotRestrictedApprenticeshipsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenValidationFails_ThenReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprn>> validatorMock,
        [Greedy] ProviderRestrictedCoursesController sut,
        int ukprn)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var errors = new List<ValidationFailure>
        {
            new(nameof(GetProviderNotRestrictedApprenticeshipsQuery.Ukprn),ProviderCourseTypeRestrictionValidator.CourseTypeRestricted)
        };

        mediatorMock
            .Setup(x => x.Send(It.Is<GetProviderNotRestrictedApprenticeshipsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetProviderNotRestrictedApprenticeshipsQueryResult>(errors));

        // Act
        var result = await sut.GetNotRestrictedApprenticeships(ukprn);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
