using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Shortlists.Commands.CreateShortlist;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Commands;

public class CreateShortlistCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShortlistExists_ReturnsExistingShortlist(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistWriteRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        Shortlist shortlist,
        CancellationToken cancellationToken)
    {
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync(shortlist);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.Shortlist.Should().Be(shortlist);
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.IsAny<Shortlist>(), cancellationToken), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShortlistDoesNotExist_CreatesNewShortlist(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistWriteRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        CancellationToken cancellationToken)
    {
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync((Shortlist)null);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.Shortlist.Should().BeEquivalentTo(command, config => config.ExcludingMissingMembers());
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.Is<Shortlist>(s => s.Id != Guid.Empty && s.UserId == command.UserId && s.Ukprn == command.Ukprn && s.LarsCode == command.LarsCode && s.LocationDescription == command.LocationDescription), cancellationToken), Times.Once);
    }
}
