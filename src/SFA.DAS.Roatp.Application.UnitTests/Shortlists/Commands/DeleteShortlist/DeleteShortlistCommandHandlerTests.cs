using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteShortlist;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Commands.DeleteShortlist;

public class DeleteShortlistCommandHandlerTests
{
    [Test, AutoData]
    public async Task Handle_InvokesRepository(DeleteShortlistCommand command, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> repoMock = new();
        repoMock.Setup(r => r.Delete(command.ShortlistId, cancellationToken)).ReturnsAsync(1);
        DeleteShortlistCommandHandler sut = new(repoMock.Object);

        await sut.Handle(command, cancellationToken);

        repoMock.Verify(r => r.Delete(command.ShortlistId, cancellationToken), Times.Once);
    }

    [Test, AutoData]
    public async Task Handle_RowsDeleted_ReturnsTrue(DeleteShortlistCommand command, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> repoMock = new();
        repoMock.Setup(r => r.Delete(command.ShortlistId, cancellationToken)).ReturnsAsync(1);
        DeleteShortlistCommandHandler sut = new(repoMock.Object);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Success.Should().BeTrue();
    }

    [Test, AutoData]
    public async Task Handle_RowsNotDeleted_ReturnsFalse(DeleteShortlistCommand command, CancellationToken cancellationToken)
    {
        Mock<IShortlistsRepository> repoMock = new();
        repoMock.Setup(r => r.Delete(command.ShortlistId, cancellationToken)).ReturnsAsync(0);
        DeleteShortlistCommandHandler sut = new(repoMock.Object);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Success.Should().BeFalse();
    }
}
