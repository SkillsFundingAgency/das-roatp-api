using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Queries;
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
            ProviderCourse providerCourse,
            List<ProviderCourseLocation> locations,
            [Frozen] Mock<IProviderReadRepository> repoMockProvider,
            [Frozen] Mock<IProviderCourseReadRepository> repoMockProviderCourse,
            [Frozen]Mock<IProviderCourseLocationReadRepository> repoMockProviderCourseLocation,
            ProviderCourseLocationsQuery query,
            int Ukprn,
            int providerId,
            int larcode,
            int providerCourseId,
            ProviderCourseLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMockProvider.Setup(r => r.GetByUkprn(Ukprn)).ReturnsAsync(provider);

            repoMockProviderCourse.Setup(r => r.GetProviderCourse(providerId, larcode)).ReturnsAsync(providerCourse);

            repoMockProviderCourseLocation.Setup(r => r.GetAllProviderCourseLocations(providerCourseId)).ReturnsAsync(locations);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProviderCourseLocations.Count, Is.EqualTo(locations.Count));
        }
      
    }
}
