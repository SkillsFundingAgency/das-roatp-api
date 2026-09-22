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
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderRestrictedCoursesControllerTests;

public class GetRestrictedApprenticeshipsTests
{
    [Test, MoqAutoData]
    public async Task WhenRequestIsValid_ThenReturnsOk(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IValidator<IUkprn>> validatorMock,
        [Greedy] ProviderRestrictedCoursesController sut,
        GetProviderRestrictedApprenticeshipsQueryResult queryResult,
        int ukprn)
    {
        // Arrange
        validatorMock
            .Setup(v => v.ValidateAsync(It.Is<UkprnValidatorModel>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mediatorMock
            .Setup(x => x.Send(It.Is<GetProviderRestrictedApprenticeshipsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>(queryResult));

        // Act
        var result = await sut.GetRestrictedApprenticeships(ukprn);

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
        var result = await sut.GetRestrictedApprenticeships(ukprn);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        mediatorMock
            .Verify(x => x.Send(It.IsAny<GetProviderRestrictedApprenticeshipsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
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
            new(nameof(GetProviderRestrictedApprenticeshipsQuery.Ukprn),ProviderCourseTypeRestrictionValidator.CourseTypeRestricted)
        };

        mediatorMock
            .Setup(x => x.Send(It.Is<GetProviderRestrictedApprenticeshipsQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetProviderRestrictedApprenticeshipsQueryResult>(errors));

        // Act
        var result = await sut.GetRestrictedApprenticeships(ukprn);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
