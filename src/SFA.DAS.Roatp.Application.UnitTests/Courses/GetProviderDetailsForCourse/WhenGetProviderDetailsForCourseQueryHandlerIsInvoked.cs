using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    public class WhenGetProviderDetailsForCourseQueryHandlerIsInvoked
    {
        Mock<IProviderDetailsReadRepository> _providerDetailsReadRepositoryMock;
        Mock<INationalAchievementRatesReadRepository> _nationalAchievementRatesReadRepositoryMock;
        Mock<IProcessProviderCourseLocationsService> _processProviderCourseLocationsServiceMock;
        Mock<IStandardsReadRepository> _standardsReadRepositoryMock;
        GetProviderDetailsForCourseQuery _query;
        List<ProviderCourseLocationDetailsModel> _providerLocationsWithDistance;
        ProviderCourseDetailsModel _providerCourseDetailsModel;
        List<DeliveryModel> _deliveryModels;
        NationalAchievementRateModel _expectedNationalAchievementRate;

        private Task<ValidatedResponse<GetProviderDetailsForCourseQueryResult>> DoAction()
        {

            Fixture fixture = new();
            _query = fixture.Create<GetProviderDetailsForCourseQuery>();

            _providerDetailsReadRepositoryMock = new();
            _providerCourseDetailsModel = fixture.Create<ProviderCourseDetailsModel>();
            _providerDetailsReadRepositoryMock
                .Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(_query.Ukprn, _query.LarsCode, _query.Latitude, _query.Longitude))
                .ReturnsAsync(_providerCourseDetailsModel);
            _providerLocationsWithDistance = fixture.CreateMany<ProviderCourseLocationDetailsModel>().ToList();
            _providerDetailsReadRepositoryMock
                .Setup(r => r.GetProviderLocationDetailsWithDistance(_query.Ukprn, _query.LarsCode, _query.Latitude, _query.Longitude)).
                ReturnsAsync(_providerLocationsWithDistance);

            _processProviderCourseLocationsServiceMock = new();
            _deliveryModels = fixture.CreateMany<DeliveryModel>().ToList();
            _processProviderCourseLocationsServiceMock
                .Setup(x => x.ConvertProviderLocationsToDeliveryModels(_providerLocationsWithDistance))
                .Returns(_deliveryModels);

            _standardsReadRepositoryMock = new();
            var standard = fixture
                .Build<Standard>()
                .With(s => s.LarsCode, _query.LarsCode)
                .Create();
            _standardsReadRepositoryMock
                .Setup(s => s.GetStandard(_query.LarsCode))
                .ReturnsAsync(standard);

            _nationalAchievementRatesReadRepositoryMock = new();
            Func<Age, ApprenticeshipLevel, int, NationalAchievementRate> CreateNationAchievementRate = (age, level, ssa1) => fixture.Build<NationalAchievementRate>().With(n => n.Age, age).With(n => n.ApprenticeshipLevel, level).With(n => n.SectorSubjectAreaTier1, ssa1).Create();

            var correctNar = CreateNationAchievementRate(Age.AllAges, ApprenticeshipLevel.AllLevels, standard.SectorSubjectAreaTier1);
            _expectedNationalAchievementRate = correctNar;
            var nationalAchievementRates = new List<NationalAchievementRate>()
            {
                correctNar,
                CreateNationAchievementRate(Age.NineteenToTwentyThree, ApprenticeshipLevel.AllLevels, standard.SectorSubjectAreaTier1),
                CreateNationAchievementRate(Age.AllAges, ApprenticeshipLevel.Two, standard.SectorSubjectAreaTier1)
            };
            _nationalAchievementRatesReadRepositoryMock
                .Setup(x => x.GetByUkprn(_query.Ukprn))
                .ReturnsAsync(nationalAchievementRates);

            GetProviderDetailsForCourseQueryHandler sut = new(_providerDetailsReadRepositoryMock.Object, _nationalAchievementRatesReadRepositoryMock.Object, _processProviderCourseLocationsServiceMock.Object, _standardsReadRepositoryMock.Object, Mock.Of<ILogger<GetProviderDetailsForCourseQueryHandler>>());

            return sut.Handle(_query, CancellationToken.None);
        }

        [Test]
        public async Task ThenGetsStandardDetails()
        {
            await DoAction();
            _standardsReadRepositoryMock.Verify(s => s.GetStandard(_query.LarsCode));
        }

        [Test]
        public async Task ThenGetProviderDetails()
        {
            await DoAction();
            _providerDetailsReadRepositoryMock.Verify(s => s.GetProviderForUkprnAndLarsCodeWithDistance(_query.Ukprn, _query.LarsCode, _query.Latitude, _query.Longitude));
        }

        [Test]
        public async Task ThenGetProviderLocation()
        {
            await DoAction();
            _providerDetailsReadRepositoryMock.Verify(r => r.GetProviderLocationDetailsWithDistance(_query.Ukprn, _query.LarsCode, _query.Latitude, _query.Longitude));
        }

        [Test]
        public async Task ThenTransformsProviderLocationToDeliveryModels()
        {
            await DoAction();
            _processProviderCourseLocationsServiceMock.Verify(s => s.ConvertProviderLocationsToDeliveryModels(_providerLocationsWithDistance));
        }

        [Test]
        public async Task ThenGetsNationalAchievementRatesForProvider()
        {
            await DoAction();
            _nationalAchievementRatesReadRepositoryMock.Verify(r => r.GetByUkprn(_query.Ukprn));
        }

        [Test]
        public async Task ThenReturnsTheAggregatedResult()
        {
            var result = await DoAction();

            using (new AssertionScope())
            {
                result.Result.Should().NotBeNull();
                result.Result.DeliveryModels.Should().HaveCount(_deliveryModels.Count);

                result.Result.AchievementRates.Should().HaveCount(1);
                result.Result.AchievementRates[0].Should().BeEquivalentTo(_expectedNationalAchievementRate);

                result.Result.Should().BeEquivalentTo(_providerCourseDetailsModel, c => c
                    .Excluding(s => s.LegalName)
                    .Excluding(s => s.StandardContactUrl)
                    .Excluding(s => s.Distance)
                    .Excluding(s => s.Ukprn));

                result.Result.Name.Should().Be(_providerCourseDetailsModel.LegalName);
                result.Result.ContactUrl.Should().Be(_providerCourseDetailsModel.StandardContactUrl);
                result.Result.ProviderHeadOfficeDistanceInMiles.Should().Be(_providerCourseDetailsModel.Distance);
            }
        }
    }
}
