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
        public async Task Handle_NoNationalAchievementRates_ReturnsResultWithEmptyList(
            ProviderCourseDetailsModel providerCourseDetailsModel,
            [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesReadRepository> nationalAchievementRatesReadRepositoryMock,
            GetProviderDetailsForCourseQuery query,
            GetProviderDetailsForCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            nationalAchievementRatesReadRepositoryMock.Setup(x => x.GetByUkprn(It.IsAny<int>()))
                .ReturnsAsync(new List<NationalAchievementRate>());

            var response = await sut.Handle(query, cancellationToken);

            response.Result.Should().NotBeNull();
            response.Result.AchievementRates.Should().HaveCount(0);
        }

        [Test, RecursiveMoqAutoData()]
        public async Task Handle_NoProvider_Locations_ReturnsResultWithEmptyList(
          ProviderCourseDetailsModel providerCourseDetailsModel,
          [Frozen] Mock<IProviderDetailsReadRepository> providerDetailsReadRepositoryMock,
          [Frozen] Mock<IProcessProviderCourseLocationsService> processProviderCourseLocationsService,
          GetProviderDetailsForCourseQuery query,
          GetProviderDetailsForCourseQueryHandler sut,
          CancellationToken cancellationToken)
        {
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderForUkprnAndLarsCodeWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync(providerCourseDetailsModel);
            providerDetailsReadRepositoryMock.Setup(r => r.GetProviderLocationDetailsWithDistance(query.Ukprn, query.LarsCode, query.Latitude, query.Longitude)).ReturnsAsync((List<ProviderCourseLocationDetailsModel>)null);
            processProviderCourseLocationsService
                .Setup(x => x.ConvertProviderLocationsToDeliveryModels(It.IsAny<List<ProviderCourseLocationDetailsModel>>()))
                .Returns(new List<DeliveryModel>());

            var response = await sut.Handle(query, cancellationToken);

            response.Result.Should().NotBeNull();
            response.Result.DeliveryModels.Should().HaveCount(0);
        }
    }
}
