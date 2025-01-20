using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForCourse
{
    [TestFixture]
    public class GetProvidersForCourseQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<ProviderCourseSummaryModel> providerCourseDetailsSummaryModels,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            List<NationalAchievementRate> nationalAchievementRates,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
            List<DeliveryModel> deliveryModels,
            Standard standard,
            ApprenticeshipLevel level,
            GetProvidersForCourseQuery query,
            GetProvidersForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            standard.Level = (int)level;
            providerDetailsReadRepositoryMock.Setup(r => r.GetProvidersForLarsCodeWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsSummaryModels);
            standardsReadMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

            var firstProviderModel = providerCourseDetailsSummaryModels.First();
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(), It.IsAny<List<ApprenticeshipLevel>>(), It.IsAny<int>()))
             .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var response = await sut.Handle(query, cancellationToken);
            var result = response.Result;
            result.Should().NotBeNull();
            result.CourseTitle.Should().Be(standard.Title);
            result.Level.Should().Be(standard.Level);
            result.LarsCode.Should().Be(standard.LarsCode);
            result.Providers.Should().HaveCount(providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            firstProviderResult.AchievementRates.Should().HaveCountGreaterThanOrEqualTo(0);
            firstProviderResult.DeliveryModels.Should().HaveCount(deliveryModels.Count);

            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.Distance)
                .Excluding(s => s.Ukprn)
                .Excluding(s => s.ProviderId));

            firstProviderResult.Name.Should().Be(firstProviderModel.LegalName);

            firstProviderResult.ProviderHeadOfficeDistanceInMiles.Should().Be((decimal?)firstProviderModel.Distance);

            var otherProviderResult = result.Providers.First(x => x.Ukprn != firstProviderModel.Ukprn);
            otherProviderResult.AchievementRates.Should().BeEmpty();
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Level6_ReturnsResult(
            List<ProviderCourseSummaryModel> providerCourseDetailsSummaryModels,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            List<NationalAchievementRate> nationalAchievementRates,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
            List<DeliveryModel> deliveryModels,
            Standard standard,
            GetProvidersForCourseQuery query,
            GetProvidersForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            standard.Level = 6;
            providerDetailsReadRepositoryMock.Setup(r => r.GetProvidersForLarsCodeWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsSummaryModels);
            standardsReadMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

            var firstProviderModel = providerCourseDetailsSummaryModels[0];
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(), new List<ApprenticeshipLevel> { ApprenticeshipLevel.AllLevels, ApprenticeshipLevel.FourPlus }, It.IsAny<int>()))
             .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var response = await sut.Handle(query, cancellationToken);
            var result = response.Result;
            result.Should().NotBeNull();
            result.CourseTitle.Should().Be(standard.Title);
            result.Level.Should().Be(standard.Level);
            result.LarsCode.Should().Be(standard.LarsCode);
            result.Providers.Should().HaveCount(providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            firstProviderResult.AchievementRates.Should().HaveCountGreaterThanOrEqualTo(0);

            firstProviderResult.DeliveryModels.Count.Should().Be(deliveryModels.Count);

            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.Distance)
                .Excluding(s => s.Ukprn)
                .Excluding(s => s.ProviderId)
             );

            firstProviderResult.Name.Should().Be(firstProviderModel.LegalName);
            firstProviderResult.ProviderHeadOfficeDistanceInMiles.Should().Be((decimal?)firstProviderModel.Distance);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task HanHandle_NoNationalAchievementRates_ReturnsResultWithEmptyList(
            List<ProviderCourseSummaryModel> providerCourseDetailsSummaryModels,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
            List<DeliveryModel> deliveryModels,
            Standard standard,
            ApprenticeshipLevel level,
            GetProvidersForCourseQuery query,
            GetProvidersForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            standard.Level = (int)level;
            providerDetailsReadRepositoryMock.Setup(r => r.GetProvidersForLarsCodeWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsSummaryModels);
            standardsReadMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

            var firstProviderModel = providerCourseDetailsSummaryModels.First();

            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(), It.IsAny<List<ApprenticeshipLevel>>(), It.IsAny<int>()))
             .ReturnsAsync(new List<NationalAchievementRate>());
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var response = await sut.Handle(query, cancellationToken);
            var result = response.Result;
            result.Should().NotBeNull();
            result.CourseTitle.Should().Be(standard.Title);
            result.Level.Should().Be(standard.Level);
            result.LarsCode.Should().Be(standard.LarsCode);
            result.Providers.Should().HaveCount(providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            firstProviderResult.AchievementRates.Should().BeEmpty();

            firstProviderResult.DeliveryModels.Count.Should().Be(deliveryModels.Count);

            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
             .Excluding(s => s.LegalName)
             .Excluding(s => s.Distance)
             .Excluding(s => s.Ukprn)
             .Excluding(s => s.ProviderId)
             );

            firstProviderResult.Name.Should().Be(firstProviderModel.LegalName);
            firstProviderResult.ProviderHeadOfficeDistanceInMiles.Should().Be((decimal?)firstProviderModel.Distance);

            var otherProviderResult = result.Providers.First(x => x.Ukprn != firstProviderModel.Ukprn);
            otherProviderResult.AchievementRates.Should().BeEmpty();
        }
    }
}
