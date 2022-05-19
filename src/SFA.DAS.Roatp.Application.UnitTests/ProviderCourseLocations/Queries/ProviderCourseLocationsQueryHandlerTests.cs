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
            List<ProviderCourseLocation> locations, 
            [Frozen]Mock<IProviderCourseLocationReadRepository> repoMock,
            ProviderCourseLocationsQuery query,
            ProviderCourseLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderCourseLocations(query.ProviderCourseId)).ReturnsAsync(locations);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProviderCourseLocations.Count, Is.EqualTo(locations.Count));
        }

        [Test, MoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IProviderCourseLocationReadRepository> repoMock,
            ProviderCourseLocationsQuery query,
            ProviderCourseLocationsQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderCourseLocations(query.ProviderCourseId)).ReturnsAsync(new List<ProviderCourseLocation>());

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProviderCourseLocations, Is.Empty);
        }
    }
}
