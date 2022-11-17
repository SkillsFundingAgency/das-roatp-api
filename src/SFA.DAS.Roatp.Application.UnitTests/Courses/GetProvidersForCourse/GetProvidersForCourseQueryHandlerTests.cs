using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using FluentAssertions;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForCourse
{
    [TestFixture]
    public class GetProvidersForCourseQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<ProviderCourseDetailsSummaryModel> providerCourseDetailsSummaryModels,
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
            providerDetailsReadRepositoryMock.Setup(r => r.GetProvidersForLarsCodeWithDistance( query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsSummaryModels);
            standardsReadMock.Setup(x => x.GetStandard(query.LarsCode)).ReturnsAsync(standard);

            var firstProviderModel = providerCourseDetailsSummaryModels.First();
            foreach (var rate in nationalAchievementRates)
            {
             rate.ProviderId=firstProviderModel.ProviderId;
            }
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(),It.IsAny<List<ApprenticeshipLevel>>(),It.IsAny<string>()))
             .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance( query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.CourseTitle,standard.Title);
            Assert.AreEqual(result.Level,standard.Level);
            Assert.AreEqual(result.LarsCode, standard.LarsCode);
            Assert.AreEqual(result.Providers.Count,providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            Assert.GreaterOrEqual(1,firstProviderResult.AchievementRates.Count);
            Assert.AreEqual(firstProviderResult.DeliveryModels.Count, deliveryModels.Count);

            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
             .Excluding(s => s.LegalName)
             .Excluding(s => s.StandardContactUrl)
             .Excluding(s => s.Distance)
             .Excluding(s => s.Ukprn)
             .Excluding(s => s.ProviderId)
             );

            Assert.AreEqual(firstProviderResult.Name, firstProviderModel.LegalName);
            Assert.AreEqual(firstProviderResult.ContactUrl, firstProviderModel.StandardContactUrl);
            Assert.AreEqual(firstProviderResult.ProviderHeadOfficeDistanceInMiles, firstProviderModel.Distance);

            var otherProviderResult = result.Providers.First(x => x.Ukprn != firstProviderModel.Ukprn);
            Assert.AreEqual(0,otherProviderResult.AchievementRates.Count);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_Level6_ReturnsResult(
            List<ProviderCourseDetailsSummaryModel> providerCourseDetailsSummaryModels,
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

            var firstProviderModel = providerCourseDetailsSummaryModels.First();
            foreach (var rate in nationalAchievementRates)
            {
                rate.ProviderId = firstProviderModel.ProviderId;
            }
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(), new List<ApprenticeshipLevel>{ ApprenticeshipLevel.AllLevels, ApprenticeshipLevel.FourPlus}, It.IsAny<string>()))
             .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.CourseTitle, standard.Title);
            Assert.AreEqual(result.Level, standard.Level);
            Assert.AreEqual(result.LarsCode, standard.LarsCode);
            Assert.AreEqual(result.Providers.Count, providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            Assert.GreaterOrEqual(1, firstProviderResult.AchievementRates.Count);

            Assert.AreEqual(firstProviderResult.DeliveryModels.Count, deliveryModels.Count);


            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
             .Excluding(s => s.LegalName)
             .Excluding(s => s.StandardContactUrl)
             .Excluding(s => s.Distance)
             .Excluding(s => s.Ukprn)
             .Excluding(s => s.ProviderId)
             );

            Assert.AreEqual(firstProviderResult.Name, firstProviderModel.LegalName);
            Assert.AreEqual(firstProviderResult.ContactUrl, firstProviderModel.StandardContactUrl);
            Assert.AreEqual(firstProviderResult.ProviderHeadOfficeDistanceInMiles, firstProviderModel.Distance);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task HanHandle_NoNationalAchievementRates_ReturnsResultWithEmptyList(
            List<ProviderCourseDetailsSummaryModel> providerCourseDetailsSummaryModels,
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
            
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByProvidersLevelsSectorSubjectArea(It.IsAny<List<int>>(), It.IsAny<List<ApprenticeshipLevel>>(), It.IsAny<string>()))
             .ReturnsAsync(new List<NationalAchievementRate>());
            providerDetailsReadRepositoryMock.Setup(r => r.GetAllProviderlocationDetailsWithDistance(query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
             .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
             .Returns(deliveryModels);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.CourseTitle, standard.Title);
            Assert.AreEqual(result.Level, standard.Level);
            Assert.AreEqual(result.LarsCode, standard.LarsCode);
            Assert.AreEqual(result.Providers.Count, providerCourseDetailsSummaryModels.Count);

            var firstProviderResult = result.Providers.First(x => x.Ukprn == firstProviderModel.Ukprn);

            Assert.AreEqual(0, firstProviderResult.AchievementRates.Count);

            Assert.AreEqual(firstProviderResult.DeliveryModels.Count, deliveryModels.Count);

            firstProviderResult.Should().BeEquivalentTo(firstProviderModel, c => c
             .Excluding(s => s.LegalName)
             .Excluding(s => s.StandardContactUrl)
             .Excluding(s => s.Distance)
             .Excluding(s => s.Ukprn)
             .Excluding(s => s.ProviderId)
             );

            Assert.AreEqual(firstProviderResult.Name, firstProviderModel.LegalName);
            Assert.AreEqual(firstProviderResult.ContactUrl, firstProviderModel.StandardContactUrl);
            Assert.AreEqual(firstProviderResult.ProviderHeadOfficeDistanceInMiles, firstProviderModel.Distance);

            var otherProviderResult = result.Providers.First(x => x.Ukprn != firstProviderModel.Ukprn);
            Assert.AreEqual(0, otherProviderResult.AchievementRates.Count);
        }
    }
}
