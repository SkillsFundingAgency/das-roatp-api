using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteExpiredShortlists;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Commands.DeleteExpiredShortlists;

public class DeleteExpiredShortlistsCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenCalled_ThenShouldDeleteExpiredShortlists()
    {
        Mock<IShortlistsRepository> repoMock = new();
        // Arrange
        var handler = new DeleteExpiredShortlistsCommandHandler(repoMock.Object);
        var command = new DeleteExpiredShortlistsCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        repoMock.Verify(x => x.DeleteExpiredShortlistItems(Application.Constants.ShortlistExpiryDays, CancellationToken.None), Times.Once);
    }
}
