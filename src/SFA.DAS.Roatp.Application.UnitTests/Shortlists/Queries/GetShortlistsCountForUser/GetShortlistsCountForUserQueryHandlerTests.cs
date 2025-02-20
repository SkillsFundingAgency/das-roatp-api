using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Queries.GetShortlistCountForUser;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Queries.GetShortlistsCountForUser;

public class GetShortlistsCountForUserQueryHandlerTests
{
    [Test, AutoData]
    public async Task Handle_ReturnsResult(Guid userId, int count, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> repoMock = new();
        repoMock.Setup(r => r.GetShortlistCount(userId, cancellationToken)).ReturnsAsync(count);

        GetShortlistsCountForUserQueryHandler sut = new(repoMock.Object);

        var result = await sut.Handle(new GetShortlistsCountForUserQuery(userId), cancellationToken);

        result.Result.Count.Should().Be(count);
    }
}
