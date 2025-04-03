using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourseProviderDetails;

public sealed class GetCourseProviderDetailsQueryResultTests
{
    [Test]
    [RecursiveMoqAutoData]
    public void ImplicitOperator_ShouldConvertCourseProviderDetailsModel_ToGetCourseProviderDetailsQueryResult(CourseProviderDetailsModel source)
    { 
        GetCourseProviderDetailsQueryResult sut = source;

        Assert.Multiple(() =>
        {
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.Ukprn, Is.EqualTo(source.Ukprn));
            Assert.That(sut.ProviderName, Is.EqualTo(source.ProviderName));
            Assert.That(sut.Address.AddressLine1, Is.EqualTo(source.MainAddressLine1));
            Assert.That(sut.Address.AddressLine2, Is.EqualTo(source.MainAddressLine2));
            Assert.That(sut.Address.AddressLine3, Is.EqualTo(source.MainAddressLine3));
            Assert.That(sut.Address.AddressLine4, Is.EqualTo(source.MainAddressLine4));
            Assert.That(sut.Address.Town, Is.EqualTo(source.MainTown));
            Assert.That(sut.Address.Postcode, Is.EqualTo(source.MainPostcode));
            Assert.That(sut.Contact.MarketingInfo, Is.EqualTo(source.MarketingInfo));
            Assert.That(sut.Contact.Website, Is.EqualTo(source.Website));
            Assert.That(sut.Contact.Email, Is.EqualTo(source.Email));
            Assert.That(sut.Contact.PhoneNumber, Is.EqualTo(source.PhoneNumber));
            Assert.That(sut.CourseName, Is.EqualTo(source.CourseName));
            Assert.That(sut.Level, Is.EqualTo(source.Level));
            Assert.That(sut.LarsCode, Is.EqualTo(source.LarsCode));
            Assert.That(sut.IFateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
            Assert.That(sut.QAR.Period, Is.EqualTo(source.Period));
            Assert.That(sut.QAR.Leavers, Is.EqualTo(source.Leavers));
            Assert.That(sut.QAR.AchievementRate, Is.EqualTo(source.AchievementRate));
            Assert.That(sut.QAR.NationalLeavers, Is.EqualTo(source.NationalLeavers));
            Assert.That(sut.QAR.NationalAchievementRate, Is.EqualTo(source.NationalAchievementRate));
            Assert.That(sut.Reviews.ReviewPeriod, Is.EqualTo(source.ReviewPeriod));
            Assert.That(sut.Reviews.EmployerReviews, Is.EqualTo(source.EmployerReviews));
            Assert.That(sut.Reviews.EmployerStars, Is.EqualTo(source.EmployerStars));
            Assert.That(sut.Reviews.EmployerRating, Is.EqualTo(source.EmployerRating));
            Assert.That(sut.Reviews.ApprenticeReviews, Is.EqualTo(source.ApprenticeReviews));
            Assert.That(sut.Reviews.ApprenticeStars, Is.EqualTo(source.ApprenticeStars));
            Assert.That(sut.Reviews.ApprenticeRating, Is.EqualTo(source.ApprenticeRating));
            Assert.That(sut.ShortlistId, Is.EqualTo(source.ShortlistId));
        });
    }
}
