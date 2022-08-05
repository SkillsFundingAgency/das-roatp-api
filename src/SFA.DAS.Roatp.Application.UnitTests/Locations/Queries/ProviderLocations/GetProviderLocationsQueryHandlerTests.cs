using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetProviderLocationsQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<ProviderLocation> locations, 
            [Frozen]Mock<IProviderLocationsReadRepository> repoMock, 
            GetProviderLocationsQuery query, 
            GetProviderLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderLocations(query.Ukprn)).ReturnsAsync(locations);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Locations.Count, Is.EqualTo(locations.Count));
        }

        [Test, MoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IProviderLocationsReadRepository> repoMock,
            GetProviderLocationsQuery query,
            GetProviderLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderLocations(query.Ukprn)).ReturnsAsync(new List<ProviderLocation>());

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Locations, Is.Empty);
        }
    }
}
