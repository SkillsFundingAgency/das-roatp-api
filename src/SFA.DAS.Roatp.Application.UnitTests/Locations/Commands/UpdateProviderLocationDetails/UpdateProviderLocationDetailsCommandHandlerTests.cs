using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.UpdateProviderLocationDetails
{
    [TestFixture]
    public class UpdateProviderLocationDetailsCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handle_NoDataFound_ThrowsInvalidOperationException(
            [Frozen] Mock<IProviderLocationsReadRepository> readRepoMock,
            UpdateProviderLocationDetailsCommandHandler sut,
            UpdateProviderLocationDetailsCommand command,
            CancellationToken cancellationToken)
        {
            readRepoMock.Setup(r => r.GetProviderLocation(It.Is<int>(i => i == command.Ukprn), It.Is<Guid>(i => i == command.Id))).ReturnsAsync((Domain.Entities.ProviderLocation)null);

            Func<Task> action = () => sut.Handle(command, cancellationToken);

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_DataFound_SaveChanges(
            [Frozen] Mock<IProviderLocationsReadRepository> readRepoMock,
            [Frozen] Mock<IProviderLocationsWriteRepository> editRepoMock,
            UpdateProviderLocationDetailsCommandHandler sut,
            UpdateProviderLocationDetailsCommand command,
            CancellationToken cancellationToken,
            Domain.Entities.ProviderLocation providerLocation)
        {
            readRepoMock.Setup(r => r.GetProviderLocation(It.Is<int>(i => i == command.Ukprn), It.Is<Guid>(i => i == command.Id))).ReturnsAsync(providerLocation);

            await sut.Handle(command, cancellationToken);

            editRepoMock.Verify(e => e.UpdateProviderlocation(It.Is<Domain.Entities.ProviderLocation>(c => c.LocationName == command.LocationName && c.Email == command.Email && c.Website == command.Website && c.Phone == command.Phone)));
        }
    }
}
