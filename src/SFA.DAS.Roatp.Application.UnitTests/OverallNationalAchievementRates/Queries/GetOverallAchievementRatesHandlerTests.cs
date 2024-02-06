using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetOverallAchievementRatesHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<NationalAchievementRateOverall> nationalAchievementRatesOverall,
            [Frozen] Mock<INationalAchievementRatesOverallReadRepository> repoMock,
            GetOverallAchievementRatesQuery query,
            GetOverallAchievementRatesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetBySectorSubjectArea(It.IsAny<int>())).ReturnsAsync(nationalAchievementRatesOverall);

            var result = await sut.Handle(query, cancellationToken);

            repoMock.Verify(d => d.GetBySectorSubjectArea(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OverallAchievementRates.Count, Is.EqualTo(nationalAchievementRatesOverall.Count));
        }

        [Test, MoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<INationalAchievementRatesOverallReadRepository> repoMock,
            GetOverallAchievementRatesQuery query,
            GetOverallAchievementRatesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetBySectorSubjectArea(It.IsAny<int>())).ReturnsAsync(new List<NationalAchievementRateOverall>());

            var result = await sut.Handle(query, cancellationToken);

            repoMock.Verify(d => d.GetBySectorSubjectArea(It.IsAny<int>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OverallAchievementRates, Is.Empty);
        }
    }
}
