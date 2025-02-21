using System;
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
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ShortlistsControllerTests;

public class ShortlistsControllerGetCountTests
{
    [Test, AutoData]
    public async Task GetShortlistsCount_ReturnsBadRequest(List<ValidationFailure> errors, CancellationToken cancellationToken)
    {
        var userId = Guid.Empty;
        Mock<IMediator> mediatorMock = new();
        ValidatedResponse<GetShortlistsCountForUserQueryResult> validatedResponse = new(errors);
        GetShortlistsCountForUserQuery query = new(userId);
        mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(validatedResponse);

        ShortlistsController sut = new(mediatorMock.Object);

        var response = await sut.GetShortlistsCountForUser(userId, cancellationToken);

        response.As<BadRequestObjectResult>().Should().NotBeNull();
    }

    [Test, AutoData]
    public async Task GetShortlistsCount_ReturnsOk(Guid userId, GetShortlistsCountForUserQueryResult result, CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        ValidatedResponse<GetShortlistsCountForUserQueryResult> validatedResponse = new(result);
        GetShortlistsCountForUserQuery query = new(userId);
        mediatorMock.Setup(m => m.Send(query, cancellationToken)).ReturnsAsync(validatedResponse);

        ShortlistsController sut = new(mediatorMock.Object);

        var response = await sut.GetShortlistsCountForUser(userId, cancellationToken);

        response.As<OkObjectResult>().Should().NotBeNull();
        response.As<OkObjectResult>().Value.As<GetShortlistsCountForUserQueryResult>().Should().Be(result);
    }
}
