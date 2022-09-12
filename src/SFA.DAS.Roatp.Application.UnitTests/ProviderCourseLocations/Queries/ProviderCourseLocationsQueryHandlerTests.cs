using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries.GetProviderCourseLocations;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Queries
{
    [TestFixture]
    public class ProviderCourseLocationsQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            Provider provider,
            Domain.Entities.ProviderCourse providerCourse,
            List<ProviderCourseLocation> locations,
            [Frozen] Mock<IProvidersReadRepository> repoMockProvider,
            [Frozen] Mock<IProviderCourseReadRepository> repoMockProviderCourse,
            [Frozen]Mock<IProviderCourseLocationsReadRepository> repoMockProviderCourseLocation,
            GetProviderCourseLocationsQuery query,
            GetProviderCourseLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMockProvider.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            repoMockProviderCourse.Setup(r => r.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourse);

            repoMockProviderCourseLocation.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(locations);

            var result = await sut.Handle(query, cancellationToken);

            repoMockProviderCourseLocation.Verify(d => d.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProviderCourseLocations.Count, Is.EqualTo(locations.Count));
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyList(
           Provider provider,
           Domain.Entities.ProviderCourse providerCourse,
           [Frozen] Mock<IProvidersReadRepository> repoMockProvider,
           [Frozen] Mock<IProviderCourseReadRepository> repoMockProviderCourse,
           [Frozen] Mock<IProviderCourseLocationsReadRepository> repoMockProviderCourseLocation,
           GetProviderCourseLocationsQuery query,
           GetProviderCourseLocationsQueryHandler sut,
           CancellationToken cancellationToken)
        {
            var locations = new List<ProviderCourseLocation>();
            repoMockProvider.Setup(r => r.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            repoMockProviderCourse.Setup(r => r.GetProviderCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(providerCourse);

            repoMockProviderCourseLocation.Setup(r => r.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(locations);

            var result = await sut.Handle(query, cancellationToken);

            repoMockProviderCourseLocation.Verify(d => d.GetAllProviderCourseLocations(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProviderCourseLocations, Is.Empty);
        }
    }
}
