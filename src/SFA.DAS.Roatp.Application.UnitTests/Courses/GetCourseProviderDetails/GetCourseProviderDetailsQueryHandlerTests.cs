using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryHandlerTests
{
    [Test]
    [MoqAutoData]
    public async Task Handle_Returns_GetCourseProviderDetailsQueryResult(
       [Frozen] Mock<ICourseProviderDetailsReadRepository> courseProviderDetailsReadRepository,
       [Greedy] GetCourseProviderDetailsQueryHandler sut,
       GetCourseProviderDetailsQuery query,
       List<CourseProviderDetailsModel> repositoryResult,
       int larsCode,
       CancellationToken cancellationToken
    )
    {
        repositoryResult[0].LarsCode = larsCode.ToString();
        query.LarsCode = larsCode.ToString();
        courseProviderDetailsReadRepository.Setup(r =>
            r.GetCourseProviderDetails(
                It.Is<GetCourseProviderDetailsParameters>(a =>
                    a.LarsCode == query.LarsCode &&
                    a.Ukprn == query.Ukprn &&
                    a.Lat == query.Latitude &&
                    a.Lon == query.Longitude &&
                    a.Location == query.Location &&
                    a.ShortlistUserId == query.ShortlistUserId
                ),
                cancellationToken
            )
        ).ReturnsAsync(repositoryResult);

        var response = await sut.Handle(query, cancellationToken);

        var result = response.Result;

        var source = repositoryResult[0];

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Ukprn, Is.EqualTo(source.Ukprn));
            Assert.That(result.ProviderName, Is.EqualTo(source.ProviderName));
            Assert.That(result.Address.AddressLine1, Is.EqualTo(source.MainAddressLine1));
            Assert.That(result.Address.AddressLine2, Is.EqualTo(source.MainAddressLine2));
            Assert.That(result.Address.AddressLine3, Is.EqualTo(source.MainAddressLine3));
            Assert.That(result.Address.AddressLine4, Is.EqualTo(source.MainAddressLine4));
            Assert.That(result.Address.Town, Is.EqualTo(source.MainTown));
            Assert.That(result.Address.Postcode, Is.EqualTo(source.MainPostcode));
            Assert.That(result.Contact.MarketingInfo, Is.EqualTo(source.MarketingInfo));
            Assert.That(result.Contact.Website, Is.EqualTo(source.Website));
            Assert.That(result.Contact.Email, Is.EqualTo(source.Email));
            Assert.That(result.Contact.PhoneNumber, Is.EqualTo(source.PhoneNumber));
            Assert.That(result.CourseName, Is.EqualTo(source.CourseName));
            Assert.That(result.Level, Is.EqualTo(source.Level));
            Assert.That(result.LarsCode.ToString(), Is.EqualTo(source.LarsCode));
            Assert.That(result.IFateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
            Assert.That(result.QAR.Period, Is.EqualTo(source.Period));
            Assert.That(result.QAR.Leavers, Is.EqualTo(source.Leavers));
            Assert.That(result.QAR.AchievementRate, Is.EqualTo(source.AchievementRate));
            Assert.That(result.QAR.NationalLeavers, Is.EqualTo(source.NationalLeavers));
            Assert.That(result.QAR.NationalAchievementRate, Is.EqualTo(source.NationalAchievementRate));
            Assert.That(result.Reviews.ReviewPeriod, Is.EqualTo(source.ReviewPeriod));
            Assert.That(result.Reviews.EmployerReviews, Is.EqualTo(source.EmployerReviews));
            Assert.That(result.Reviews.EmployerStars, Is.EqualTo(source.EmployerStars));
            Assert.That(result.Reviews.EmployerRating, Is.EqualTo(source.EmployerRating));
            Assert.That(result.Reviews.ApprenticeReviews, Is.EqualTo(source.ApprenticeReviews));
            Assert.That(result.Reviews.ApprenticeStars, Is.EqualTo(source.ApprenticeStars));
            Assert.That(result.Reviews.ApprenticeRating, Is.EqualTo(source.ApprenticeRating));
            Assert.That(result.ShortlistId, Is.EqualTo(source.ShortlistId));
            Assert.That(result.Locations.Count(), Is.EqualTo(repositoryResult.Count));
        });
    }

    [Test]
    [MoqAutoData]
    public async Task Handle_ReturnsNull_ProviderNotFound(
       [Frozen] Mock<ICourseProviderDetailsReadRepository> courseProviderDetailsReadRepository,
       [Greedy] GetCourseProviderDetailsQueryHandler sut,
       GetCourseProviderDetailsQuery query,
       CancellationToken cancellationToken
   )
    {
        courseProviderDetailsReadRepository.Setup(r =>
            r.GetCourseProviderDetails(
                It.Is<GetCourseProviderDetailsParameters>(a =>
                    a.LarsCode == query.LarsCode &&
                    a.Ukprn == query.Ukprn &&
                    a.Lat == query.Latitude &&
                    a.Lon == query.Longitude &&
                    a.Location == query.Location &&
                    a.ShortlistUserId == query.ShortlistUserId
                ),
                cancellationToken
            )
        ).ReturnsAsync(new List<CourseProviderDetailsModel>());

        var response = await sut.Handle(query, cancellationToken);
        Assert.That(response.Result, Is.Null);
    }
}
