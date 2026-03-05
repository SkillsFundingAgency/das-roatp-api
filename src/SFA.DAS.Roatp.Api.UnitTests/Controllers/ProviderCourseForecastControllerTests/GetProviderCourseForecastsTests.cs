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
using SFA.DAS.Roatp.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderCourseForecastControllerTests;

public class GetProviderCourseForecastsTests
{
    [Test, MoqAutoData]
    public async Task GetProviderCourseForecasts_ReturnsOk(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderCourseForecastsController _sut,
        int ukprn,
        string larsCode,
        GetProviderCourseForecastsQueryResult expectedResult,
        CancellationToken cancellationToken)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseForecastsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode), cancellationToken)).ReturnsAsync(new ValidatedResponse<GetProviderCourseForecastsQueryResult>(expectedResult));
        // Act
        IActionResult actionResult = await _sut.GetProviderCourseForecasts(ukprn, larsCode, cancellationToken);
        // Assert
        actionResult.As<OkObjectResult>().Value.As<GetProviderCourseForecastsQueryResult>().Should().Be(expectedResult);
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourseForecasts_ReturnsBadRequest(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderCourseForecastsController _sut,
        int ukprn,
        string larsCode,
        IEnumerable<ValidationFailure> errors,
        CancellationToken cancellationToken)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseForecastsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode), cancellationToken)).ReturnsAsync(new ValidatedResponse<GetProviderCourseForecastsQueryResult>(errors));
        // Act
        IActionResult actionResult = await _sut.GetProviderCourseForecasts(ukprn, larsCode, cancellationToken);
        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourseForecasts_ReturnsNotFound(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderCourseForecastsController _sut,
        int ukprn,
        string larsCode,
        CancellationToken cancellationToken)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseForecastsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode), cancellationToken)).ReturnsAsync(new ValidatedResponse<GetProviderCourseForecastsQueryResult>((GetProviderCourseForecastsQueryResult)null));
        // Act
        IActionResult actionResult = await _sut.GetProviderCourseForecasts(ukprn, larsCode, cancellationToken);
        // Assert
        actionResult.Should().BeOfType<NotFoundResult>();
    }
}
