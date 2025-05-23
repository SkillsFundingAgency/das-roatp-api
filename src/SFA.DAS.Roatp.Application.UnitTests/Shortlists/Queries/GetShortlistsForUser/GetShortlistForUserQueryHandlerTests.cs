﻿using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistsForUser;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Queries.GetShortlistsForUser;

public class GetShortlistForUserQueryHandlerTests
{
    [Test, AutoData]
    public async Task Handle_UserHasShortlistItems_ReturnsShortlistDetails(GetShortlistsForUserQueryResult expected, Guid userId, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> shortlistsRepoMock = new();
        shortlistsRepoMock.Setup(x => x.GetShortlistCount(userId, cancellationToken)).ReturnsAsync(1);
        shortlistsRepoMock.Setup(x => x.GetShortlistsForUser(userId, cancellationToken)).ReturnsAsync(JsonSerializer.Serialize(expected, GetShortlistForUserQueryHandler.SerializerOptions));
        GetShortlistForUserQueryHandler sut = new(shortlistsRepoMock.Object);

        ValidatedResponse<GetShortlistsForUserQueryResult> validatedResponse = await sut.Handle(new GetShortlistsForUserQuery(userId), cancellationToken);

        shortlistsRepoMock.Verify(x => x.GetShortlistsForUser(userId, cancellationToken), Times.Once);
        validatedResponse.Result.Should().BeEquivalentTo(expected);
    }

    [Test, AutoData]
    public async Task Handle_UserHasNoShortlistItems_ReturnsEmptyResponse(Guid userId, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> shortlistsRepoMock = new();
        shortlistsRepoMock.Setup(x => x.GetShortlistCount(userId, cancellationToken)).ReturnsAsync(0);
        GetShortlistForUserQueryHandler sut = new(shortlistsRepoMock.Object);

        ValidatedResponse<GetShortlistsForUserQueryResult> validatedResponse = await sut.Handle(new GetShortlistsForUserQuery(userId), cancellationToken);

        shortlistsRepoMock.Verify(x => x.GetShortlistsForUser(userId, cancellationToken), Times.Never);
        validatedResponse.Result.Courses.Should().BeEmpty();
        validatedResponse.Result.UserId.Should().Be(userId);
    }
}
