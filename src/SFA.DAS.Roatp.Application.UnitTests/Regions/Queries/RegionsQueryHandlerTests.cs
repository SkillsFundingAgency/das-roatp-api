using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Regions.Queries;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class RegionsQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<Domain.Entities.Region> regions, 
            [Frozen]Mock<IRegionsReadRepository> repoMock, 
            RegionsQuery query,
            RegionsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllRegions()).ReturnsAsync(regions);

            var result = await sut.Handle(query, cancellationToken);

            repoMock.Verify(d => d.GetAllRegions(), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Regions.Count, Is.EqualTo(regions.Count));
        }

        [Test, MoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IRegionsReadRepository> repoMock,
            RegionsQuery query,
            RegionsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllRegions()).ReturnsAsync(new List<Domain.Entities.Region>());

            var result = await sut.Handle(query, cancellationToken);

            repoMock.Verify(d => d.GetAllRegions(), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Regions, Is.Empty);
        }
    }
}
