using System.Threading;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ShortlistsControllerTests;

public class ShortlistsControllerGetShortlistsForUserTests
{
    [Test, AutoData]
    public void GetShortlistsForUser_ReturnsBadRequest(
        GetShortlistsForUserQuery query,
        CancellationToken cancellationToken)
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var validatedResponse = new ValidatedResponse<GetShortlistsForUserQueryResult>([new ValidationFailure("userId", "invalid")]);
        mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(validatedResponse);
        var sut = new ShortlistsController(mediatorMock.Object);
        // Act
        var response = sut.GetShortlistsForUser(query.UserId, cancellationToken).Result;
        // Assert
        response.As<BadRequestObjectResult>().Should().NotBeNull();
    }

    [Test, AutoData]
    public void GetShortlistsForUser_ReturnsOkResult(
        GetShortlistsForUserQuery query,
        GetShortlistsForUserQueryResult result,
        CancellationToken cancellationToken)
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var validatedResponse = new ValidatedResponse<GetShortlistsForUserQueryResult>(result);
        mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(validatedResponse);
        var sut = new ShortlistsController(mediatorMock.Object);
        // Act
        var response = sut.GetShortlistsForUser(query.UserId, cancellationToken).Result;
        // Assert
        response.As<OkObjectResult>().Should().NotBeNull();
        response.As<OkObjectResult>().Value.As<GetShortlistsForUserQueryResult>().Should().Be(result);
    }
}
