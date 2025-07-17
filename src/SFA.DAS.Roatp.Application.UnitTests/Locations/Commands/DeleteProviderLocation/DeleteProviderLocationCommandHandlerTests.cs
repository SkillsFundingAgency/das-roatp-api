using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.DeleteProviderLocation;


[TestFixture]
public class DeleteProviderLocationCommandHandlerTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task Handle_Deletes_Record(
        [Frozen] Mock<IProviderLocationsWriteRepository> providerLocationDeleteRepositoryMock,
        DeleteProviderLocationCommand command,
        DeleteProviderLocationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        var response = await sut.Handle(command, cancellationToken);

        providerLocationDeleteRepositoryMock.Verify(d => d.Delete(
            It.Is<int>(ukprn => ukprn == command.Ukprn),
            It.Is<Guid>(id => id == command.Id),
            It.Is<string>(userId => userId == command.UserId),
            It.Is<string>(username => username == command.UserDisplayName),
            It.Is<string>(type => type == AuditEventTypes.DeleteProviderLocation)),
            Times.Once);

        response.Result.Should().Be(true);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task Handle_Finds_No_Id_Match_Returns_NotFound(
        [Frozen] Mock<IProviderLocationsWriteRepository> providerLocationDeleteRepositoryMock,
        [Frozen] Mock<IProviderLocationsReadRepository> providerLocationReadRepositoryMock,
        DeleteProviderLocationCommand command,
        DeleteProviderLocationCommandHandler sut,
        CancellationToken cancellationToken)
    {
        providerLocationReadRepositoryMock.Setup(x => x.GetProviderLocation(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync((ProviderLocation)null);

        var response = await sut.Handle(command, cancellationToken);

        providerLocationDeleteRepositoryMock.Verify(d => d.Delete(
                It.IsAny<int>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        response.Result.Should().Be(false);
    }
}