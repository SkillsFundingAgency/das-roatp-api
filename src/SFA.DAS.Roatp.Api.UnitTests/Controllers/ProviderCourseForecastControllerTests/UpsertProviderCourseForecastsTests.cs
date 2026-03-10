using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderCourseForecastControllerTests;

public class UpsertProviderCourseForecastsTests
{
    [Test, MoqAutoData]
    public async Task UpsertProviderCourseForecasts_ReturnsNoContent(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderCourseForecastsController _sut,
        int ukprn,
        string larsCode,
        IEnumerable<UpsertProviderCourseForecastModel> forecasts,
        CancellationToken cancellationToken)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.Is<UpsertProviderCourseForecastsCommand>(q => q.Ukprn == ukprn && q.LarsCode == larsCode && q.Forecasts == forecasts), cancellationToken)).ReturnsAsync(ValidatedResponse.Valid());
        // Act
        IActionResult actionResult = await _sut.UpsertProviderCourseForecasts(ukprn, larsCode, forecasts, cancellationToken);
        // Assert
        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Test, MoqAutoData]
    public async Task UpsertProviderCourseForecasts_ReturnsBadRequest(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderCourseForecastsController _sut,
        int ukprn,
        string larsCode,
        IEnumerable<UpsertProviderCourseForecastModel> forecasts,
        IEnumerable<ValidationFailure> errors,
        CancellationToken cancellationToken)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.Is<UpsertProviderCourseForecastsCommand>(q => q.Ukprn == ukprn && q.LarsCode == larsCode && q.Forecasts == forecasts), cancellationToken)).ReturnsAsync(new ValidatedResponse(errors));
        // Act
        IActionResult actionResult = await _sut.UpsertProviderCourseForecasts(ukprn, larsCode, forecasts, cancellationToken);
        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }
}
