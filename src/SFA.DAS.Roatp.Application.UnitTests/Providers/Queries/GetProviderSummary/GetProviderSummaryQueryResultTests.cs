using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderSummary;

[TestFixture]
public class GetProviderSummaryQueryResultTests
{
    [Test, MoqAutoData]
    public void Operator_Populates_Response_From_Model(ProviderSummaryModel source)
    {
        var sut = (GetProviderSummaryQueryResult)source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.Ukprn, Is.EqualTo(source.Ukprn));
            Assert.That(sut.Name, Is.EqualTo(source.LegalName));
            Assert.That(sut.TradingName, Is.EqualTo(source.TradingName));
            Assert.That(sut.Email, Is.EqualTo(source.Email));
            Assert.That(sut.Phone, Is.EqualTo(source.Phone));
            Assert.That(sut.ContactUrl, Is.EqualTo(source.ContactUrl));
            Assert.That(sut.ProviderTypeId, Is.EqualTo(source.ProviderTypeId));
            Assert.That(sut.StatusId, Is.EqualTo(source.StatusId));
            Assert.That(sut.MarketingInfo, Is.EqualTo(source.MarketingInfo));
            Assert.That(sut.CanAccessApprenticeshipService, Is.EqualTo(source.CanAccessApprenticeshipService));
            Assert.That(sut.Address.AddressLine1, Is.EqualTo(source.MainAddressLine1));
            Assert.That(sut.Address.AddressLine2, Is.EqualTo(source.MainAddressLine2));
            Assert.That(sut.Address.AddressLine3, Is.EqualTo(source.MainAddressLine3));
            Assert.That(sut.Address.AddressLine4, Is.EqualTo(source.MainAddressLine4));
            Assert.That(sut.Address.Town, Is.EqualTo(source.MainTown));
            Assert.That(sut.Address.Postcode, Is.EqualTo(source.MainPostcode));
            Assert.That(sut.Address.Latitude, Is.EqualTo(source.Latitude));
            Assert.That(sut.Address.Longitude, Is.EqualTo(source.Longitude));
            Assert.That(sut.Qar.Period, Is.EqualTo(source.QARPeriod));
            Assert.That(sut.Qar.Leavers, Is.EqualTo(source.Leavers));
            Assert.That(sut.Qar.AchievementRate, Is.EqualTo(source.AchievementRate));
            Assert.That(sut.Qar.NationalAchievementRate, Is.EqualTo(source.NationalAchievementRate));
            Assert.That(sut.Reviews.ReviewPeriod, Is.EqualTo(source.ReviewPeriod));
            Assert.That(sut.Reviews.EmployerReviews, Is.EqualTo(source.EmployerReviews));
            Assert.That(sut.Reviews.EmployerStars, Is.EqualTo(source.EmployerStars));
            Assert.That(sut.Reviews.EmployerRating, Is.EqualTo(source.EmployerRating));
            Assert.That(sut.Reviews.ApprenticeReviews, Is.EqualTo(source.ApprenticeReviews));
            Assert.That(sut.Reviews.ApprenticeStars, Is.EqualTo(source.ApprenticeStars));
            Assert.That(sut.Reviews.ApprenticeRating, Is.EqualTo(source.ApprenticeRating));
        });
    }
}
