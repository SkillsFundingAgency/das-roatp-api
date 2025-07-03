using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using SFA.DAS.Roatp.Domain.Constants;
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

        response.Result.Should().NotBeNull();
        response.Result.Should().Be(Unit.Value);
    }
}