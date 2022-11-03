﻿using System.Collections.Generic;
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
            GetProviderDetailsForCourseQuery query,
            GetProviderDetailsForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude,query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(providerCourseDetailsModel.Ukprn))
                .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.AchievementRates.Count,nationalAchievementRates.Count);
            Assert.AreEqual(result.LocationDetails.Count,providerLocationsWithDistance.Count);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s=>s.Distance)
                );

            Assert.AreEqual(result.Name, providerCourseDetailsModel.LegalName);
            Assert.AreEqual(result.ContactUrl, providerCourseDetailsModel.StandardContactUrl);
           Assert.AreEqual(result.ProviderHeadOfficeDistanceInMiles,providerCourseDetailsModel.Distance);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoNationalAchievementRates_ReturnsResultWithEmptyList(
            ProviderCourseDetailsModel providerCourseDetailsModel,
            List<ProviderCourseLocationDetailsModel> providerLocationsWithDistance,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            GetProviderDetailsForCourseQuery query,
            GetProviderDetailsForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync((List<NationalAchievementRate>)null);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerLocationsWithDistance);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.AchievementRates.Count, 0);
            Assert.AreEqual(result.LocationDetails.Count, providerLocationsWithDistance.Count);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
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
          GetProviderDetailsForCourseQuery query,
          GetProviderDetailsForCourseQueryHandler sut,
          CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(providerCourseDetailsModel.Ukprn))
                .ReturnsAsync(nationalAchievementRates);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderlocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync((List<ProviderCourseLocationDetailsModel>)null);

            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.AchievementRates.Count, nationalAchievementRates.Count);
            Assert.AreEqual(result.LocationDetails.Count, 0);

            result.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                );

            Assert.AreEqual(result.Name, providerCourseDetailsModel.LegalName);
            Assert.AreEqual(result.ContactUrl, providerCourseDetailsModel.StandardContactUrl);
            Assert.AreEqual(result.ProviderHeadOfficeDistanceInMiles, providerCourseDetailsModel.Distance);
        }
    }
}
