using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkDelete
{
    [TestFixture]
    public class DeleteUnmatchedRegionalProviderLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_Records(
           [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
           [Frozen] Mock<IProviderLocationsDeleteRepository> providerLocationsDeleteRepositoryMock,
           DeleteUnmatchedRegionalProviderLocationsCommand command,
           DeleteUnmatchedRegionalProviderLocationsCommandHandler sut,
           CancellationToken cancellationToken)
        {
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation { Id = 1, ProviderId = 1, LocationType = LocationType.Provider},
                new ProviderLocation { Id = 2, ProviderId = 1, LocationType = LocationType.Regional}
            };
           
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(providerLocations);
          
            var result = await sut.Handle(command, cancellationToken);

            providerLocationsDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_NoRecords(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsDeleteRepository> providerLocationsDeleteRepositoryMock,
            DeleteUnmatchedRegionalProviderLocationsCommand command,
            DeleteUnmatchedRegionalProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var providerLocations = new List<ProviderLocation> { new ProviderLocation { Id = 1, ProviderId = 1, LocationType = LocationType.Provider} };
         
            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(providerLocations);
           
            var result = await sut.Handle(command, cancellationToken);
        
            providerLocationsDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(0));
        }
    }
}


