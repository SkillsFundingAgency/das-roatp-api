using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_Records(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationReadRepository> providerCourseLocationReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsDeleteRepository> providerLocationsDeleteRepositoryMock,
            BulkDeleteProviderLocationsCommand command,
            BulkDeleteProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var providerId = 1;
            var regionalLocationId = 2;
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation { Id = 1, ProviderId =providerId, LocationType = LocationType.Provider},
                new ProviderLocation { Id = regionalLocationId, ProviderId = providerId, LocationType = LocationType.Regional},
                new ProviderLocation { Id = 3, ProviderId = providerId, LocationType = LocationType.National}
            };
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { Id = 1, ProviderCourseId = 2, ProviderLocationId=1 } };

            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(command.Ukprn)).ReturnsAsync(providerLocations);
            providerCourseLocationReadRepositoryMock.Setup(r => r.GetProviderCourseLocationsByUkprn(command.Ukprn)).ReturnsAsync(providerCourseLocations);

            var result = await sut.Handle(command, cancellationToken);

            providerLocationsDeleteRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(x=>x.Contains(regionalLocationId))), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Deletes_NoRecords(
            [Frozen] Mock<IProviderLocationsReadRepository> providerLocationsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationReadRepository> providerCourseLocationReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsDeleteRepository> providerLocationsDeleteRepositoryMock,
            BulkDeleteProviderLocationsCommand command,
            BulkDeleteProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var providerId = 1;
            var providerLocations = new List<ProviderLocation>
            {
                new ProviderLocation { Id = 1, ProviderId = providerId, LocationType = LocationType.Provider},
                new ProviderLocation { Id = 3, ProviderId = providerId, LocationType = LocationType.National}
            };
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { Id = 1, ProviderCourseId = 1, ProviderLocationId = 1 } };

            providerLocationsReadRepositoryMock.Setup(r => r.GetAllProviderLocations(It.IsAny<int>())).ReturnsAsync(providerLocations);
            providerCourseLocationReadRepositoryMock.Setup(r => r.GetProviderCourseLocationsByUkprn(It.IsAny<int>())).ReturnsAsync(providerCourseLocations);

            var result = await sut.Handle(command, cancellationToken);

            providerLocationsDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(0));
        }
    }

}
