﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteShortlist;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ShortlistsControllerTests;

public class ShortlistsControllerDeleteTests
{
    [Test, AutoData]
    public async Task DeleteShortlist_ReturnsAcceptedResponse(Guid shortlistId, bool expected, CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        ShortlistsController sut = new(mediatorMock.Object);
        mediatorMock.Setup(m => m.Send(It.IsAny<DeleteShortlistCommand>(), cancellationToken)).ReturnsAsync(new DeleteShortlistCommandResult(expected));

        var response = await sut.DeleteShortlist(shortlistId, cancellationToken);

        mediatorMock.Verify(m => m.Send(It.Is<DeleteShortlistCommand>(c => c.ShortlistId == shortlistId), cancellationToken), Times.Once());
        response.As<AcceptedResult>().Should().NotBeNull();
        response.As<AcceptedResult>().Value.As<DeleteShortlistCommandResult>().Success.Should().Be(expected);
    }
}
