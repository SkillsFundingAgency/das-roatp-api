using System.Collections.Generic;
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

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class GetProviderDetailsForCourseQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            ProviderCourseDetailsModel providerCourseDetailsModel,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            List<NationalAchievementRate> nationalAchievementRates,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
            List<DeliveryModel> deliveryModels,
            GetProviderDetailsForCourseQuery query,
            GetProviderDetailsForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(providerCourseDetailsModel.Ukprn))
                .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
                .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
                .Returns(deliveryModels);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(0,result.AchievementRates.Count);
            Assert.AreEqual(result.DeliveryModels.Count, deliveryModels.Count);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.Ukprn)
                );

            Assert.AreEqual(result.Name, providerCourseDetailsModel.LegalName);
            Assert.AreEqual(result.ContactUrl, providerCourseDetailsModel.StandardContactUrl);
            Assert.AreEqual(result.ProviderHeadOfficeDistanceInMiles, providerCourseDetailsModel.Distance);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoNationalAchievementRates_ReturnsResultWithEmptyList(
            ProviderCourseDetailsModel providerCourseDetailsModel,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
            List<DeliveryModel> deliveryModels,
            GetProviderDetailsForCourseQuery query,
            GetProviderDetailsForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync((List<NationalAchievementRate>)null);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);
            processProviderCourseLocationsService
                .Setup(x => x.ConvertProviderLocationsToDeliveryModels(providerLocationsWithDistance))
                .Returns(deliveryModels);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(0, result.AchievementRates.Count);
            Assert.AreEqual(result.DeliveryModels.Count, deliveryModels.Count);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.Ukprn)
            );

            Assert.AreEqual(result.Name, providerCourseDetailsModel.LegalName);
            Assert.AreEqual(result.ContactUrl, providerCourseDetailsModel.StandardContactUrl);
            Assert.AreEqual(result.ProviderHeadOfficeDistanceInMiles, providerCourseDetailsModel.Distance);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoProvider_Locations_ReturnsResultWithEmptyList(
          ProviderCourseDetailsModel providerCourseDetailsModel,
          List<NationalAchievementRate> nationalAchievementRates,
          [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
          [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
          [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
          GetProviderDetailsForCourseQuery query,
          GetProviderDetailsForCourseQueryHandler sut,
          CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(providerCourseDetailsModel.Ukprn))
                .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync((List<ProviderCourseLocationDetailsModel>)null);
            processProviderCourseLocationsService
                .Setup(x => x.ConvertProviderLocationsToDeliveryModels(It.IsAny<List<ProviderCourseLocationDetailsModel>>()))
                .Returns(new List<DeliveryModel>());

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(0, result.AchievementRates.Count);
            Assert.AreEqual(0, result.DeliveryModels.Count);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.Ukprn)
                );

            Assert.AreEqual(result.Name, providerCourseDetailsModel.LegalName);
            Assert.AreEqual(result.ContactUrl, providerCourseDetailsModel.StandardContactUrl);
            Assert.AreEqual(result.ProviderHeadOfficeDistanceInMiles, providerCourseDetailsModel.Distance);
        }
    }
}
