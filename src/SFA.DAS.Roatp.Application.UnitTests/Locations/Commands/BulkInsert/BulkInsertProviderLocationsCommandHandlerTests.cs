using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkInsert
{
    [TestFixture]
    public class BulkInsertProviderLocationsCommandHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Inserts_Records(
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IRegionsReadRepository> regionsReadRepositoryMock,
            [Frozen] Mock<IProviderLocationsBulkRepository> providerLocationsBulkRepositoryMock,
            BulkInsertProviderLocationsCommand command,
            BulkInsertProviderLocationsCommandHandler sut,
            CancellationToken cancellationToken)
        {
            var provider = new Provider { Id = 1, Ukprn = command.Ukprn };
            var providerLocations = new List<ProviderLocation> { new ProviderLocation { Id = 1, ProviderId = 1, RegionId = command.SelectedSubregionIds.First() } };
            var regions = new List<Region>() { new Region { RegionName = "Test", Id = command.SelectedSubregionIds.First(), SubregionName = "Test", Latitude=11, Longitude=10} };
            providersReadRepositoryMock.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            regionsReadRepositoryMock.Setup(r => r.GetAllRegions()).ReturnsAsync(regions);

            command.SelectedSubregionIds = new List<int> { providerLocations.FirstOrDefault(a => a.RegionId.HasValue).RegionId.Value };

            var result = await sut.Handle(command, cancellationToken);
            providerLocationsBulkRepositoryMock.Verify(d => d.BulkInsert(It.IsAny<IEnumerable<ProviderLocation>>()), Times.Once);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(1));
        }
    }

}
