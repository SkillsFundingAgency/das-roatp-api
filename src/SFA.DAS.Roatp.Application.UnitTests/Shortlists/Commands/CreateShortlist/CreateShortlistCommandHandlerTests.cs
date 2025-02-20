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

namespace SFA.DAS.Roatp.Application.UnitTests.Shortlists.Commands.CreateShortlist;

public class CreateShortlistCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShortlistExists_ReturnsExistingShortlist(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistsRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        Shortlist shortlist,
        CancellationToken cancellationToken)
    {
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync(shortlist);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.ShortlistId.Should().Be(shortlist.Id);
        actual.Result.IsCreated.Should().BeFalse();
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.IsAny<Shortlist>(), cancellationToken), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShortlistDoesNotExist_CreatesNewShortlist(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistsRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        CancellationToken cancellationToken)
    {
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync((Shortlist)null);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.IsCreated.Should().BeTrue();
        actual.Result.ShortlistId.Should().NotBeEmpty();
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.Is<Shortlist>(s => s.Id != Guid.Empty && s.UserId == command.UserId && s.Ukprn == command.Ukprn && s.LarsCode == command.LarsCode && s.LocationDescription == command.LocationDescription && s.CreatedDate != DateTime.MinValue), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShortlistDoesNotExist_LocationNotGiven_CreatesNewShortlistWithoutCoordinates(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistsRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.LocationDescription = null;
        command.Latitude = 1;
        command.Longitude = 2;
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync((Shortlist)null);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.IsCreated.Should().BeTrue();
        actual.Result.ShortlistId.Should().NotBeEmpty();
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.Is<Shortlist>(s => s.Id != Guid.Empty && s.UserId == command.UserId && s.Ukprn == command.Ukprn && s.LarsCode == command.LarsCode && s.LocationDescription == null && s.Latitude == null && s.Longitude == null), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShortlistDoesNotExist_LocationGiven_CreatesNewShortlistWithCoordinates(
        CreateShortlistCommand command,
        [Frozen] Mock<IShortlistsRepository> shortlistWriteRepositoryMock,
        CreateShortlistCommandHandler sut,
        CancellationToken cancellationToken)
    {
        command.LocationDescription = "MK4 4ET";
        command.Longitude = 1;
        command.Latitude = 2;
        shortlistWriteRepositoryMock.Setup(s => s.Get(command.UserId, command.Ukprn, command.LarsCode, command.LocationDescription, cancellationToken)).ReturnsAsync((Shortlist)null);

        var actual = await sut.Handle(command, cancellationToken);

        actual.Result.IsCreated.Should().BeTrue();
        actual.Result.ShortlistId.Should().NotBeEmpty();
        shortlistWriteRepositoryMock.Verify(s => s.Create(It.Is<Shortlist>(s => s.Id != Guid.Empty && s.UserId == command.UserId && s.Ukprn == command.Ukprn && s.LarsCode == command.LarsCode && s.LocationDescription == command.LocationDescription && s.Latitude == command.Latitude && s.Longitude == command.Longitude), cancellationToken), Times.Once);
    }
}
