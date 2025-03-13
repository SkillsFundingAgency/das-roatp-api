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
using SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ShortlistsControllerTests;
public class ShortlistsControllerCreateTests
{
    [Test, AutoData]
    public async Task CreateShortlist_ReturnsBadRequest(List<ValidationFailure> errors, CreateShortlistCommand command, CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        ValidatedResponse<CreateShortlistCommandResult> validatedResponse = new(errors);
        mediatorMock.Setup(m => m.Send(command, cancellationToken)).ReturnsAsync(validatedResponse);

        ShortlistsController sut = new(mediatorMock.Object);

        var response = await sut.CreateShortlist(command, cancellationToken);

        response.As<BadRequestObjectResult>().Should().NotBeNull();
    }

    [Test, AutoData]
    public async Task CreateShortlist_ShortlistExists_ReturnsNoContent(CreateShortlistCommand command, CancellationToken cancellationToken)
    {
        CreateShortlistCommandResult result = new() { ShortlistId = Guid.NewGuid(), IsCreated = false };
        Mock<IMediator> mediatorMock = new();
        ValidatedResponse<CreateShortlistCommandResult> validatedResponse = new(result);
        mediatorMock.Setup(m => m.Send(command, cancellationToken)).ReturnsAsync(validatedResponse);

        ShortlistsController sut = new(mediatorMock.Object);

        var response = await sut.CreateShortlist(command, cancellationToken);

        response.As<NoContentResult>().Should().NotBeNull();
    }

    [Test, AutoData]
    public async Task CreateShortlist_ShortlistDoesNotExists_ReturnsCreated(CreateShortlistCommand command, CancellationToken cancellationToken)
    {
        CreateShortlistCommandResult result = new() { ShortlistId = Guid.NewGuid(), IsCreated = true };
        Mock<IMediator> mediatorMock = new();
        ValidatedResponse<CreateShortlistCommandResult> validatedResponse = new(result);
        mediatorMock.Setup(m => m.Send(command, cancellationToken)).ReturnsAsync(validatedResponse);

        ShortlistsController sut = new(mediatorMock.Object);

        var response = await sut.CreateShortlist(command, cancellationToken);

        response.As<CreatedResult>().Should().NotBeNull();

        response.As<CreatedResult>().Location.Should().Be($"/users/{command.UserId}");
    }
}
