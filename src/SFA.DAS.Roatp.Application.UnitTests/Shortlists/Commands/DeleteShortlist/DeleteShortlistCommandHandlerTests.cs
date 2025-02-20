using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
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
        DeleteShortlistCommandHandler sut = new(repoMock.Object);

        await sut.Handle(command, cancellationToken);

        repoMock.Verify(r => r.Delete(command.ShortlistId, cancellationToken), Times.Once);
    }
}
