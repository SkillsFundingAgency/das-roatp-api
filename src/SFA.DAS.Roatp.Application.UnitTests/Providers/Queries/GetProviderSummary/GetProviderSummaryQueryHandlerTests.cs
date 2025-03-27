using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderSummary;

[TestFixture]
public class GetProviderSummaryQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_CorrectlyReturns_GetProviderSummaryQueryResult(
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> _providersRegistrationDetailReadRepositoryMock,
        [Frozen] GetProviderSummaryQuery query,
        GetProviderSummaryQueryHandler sut,
        ProviderSummaryModel providerSummaryModel,
        CancellationToken cancellationToken
    )
    {
        _providersRegistrationDetailReadRepositoryMock.Setup(r => 
            r.GetProviderSummary(query.Ukprn, CancellationToken.None)
        ).ReturnsAsync(providerSummaryModel);

        var response = await sut.Handle(query, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);

            GetProviderSummaryQueryResult result = response.Result;

            Assert.That(result.Ukprn, Is.EqualTo(providerSummaryModel.Ukprn));
            Assert.That(result.Name, Is.EqualTo(providerSummaryModel.LegalName));
            Assert.That(result.TradingName, Is.EqualTo(providerSummaryModel.TradingName));
            Assert.That(result.Email, Is.EqualTo(providerSummaryModel.Email));
            Assert.That(result.Phone, Is.EqualTo(providerSummaryModel.Phone));
            Assert.That(result.ContactUrl, Is.EqualTo(providerSummaryModel.ContactUrl));
            Assert.That(result.ProviderTypeId, Is.EqualTo(providerSummaryModel.ProviderTypeId));
            Assert.That(result.StatusId, Is.EqualTo(providerSummaryModel.StatusId));
            Assert.That(result.MarketingInfo, Is.EqualTo(providerSummaryModel.MarketingInfo));
            Assert.That(result.CanAccessApprenticeshipService, Is.EqualTo(providerSummaryModel.CanAccessApprenticeshipService));
            Assert.That(result.Address.AddressLine1, Is.EqualTo(providerSummaryModel.MainAddressLine1));
            Assert.That(result.Address.AddressLine2, Is.EqualTo(providerSummaryModel.MainAddressLine2));
            Assert.That(result.Address.AddressLine3, Is.EqualTo(providerSummaryModel.MainAddressLine3));
            Assert.That(result.Address.AddressLine4, Is.EqualTo(providerSummaryModel.MainAddressLine4));
            Assert.That(result.Address.Town, Is.EqualTo(providerSummaryModel.MainTown));
            Assert.That(result.Address.Postcode, Is.EqualTo(providerSummaryModel.MainPostcode));
            Assert.That(result.Address.Latitude, Is.EqualTo(providerSummaryModel.Latitude));
            Assert.That(result.Address.Longitude, Is.EqualTo(providerSummaryModel.Longitude));
            Assert.That(result.Qar.Period, Is.EqualTo(providerSummaryModel.QARPeriod));
            Assert.That(result.Qar.Leavers, Is.EqualTo(providerSummaryModel.Leavers));
            Assert.That(result.Qar.AchievementRate, Is.EqualTo(providerSummaryModel.AchievementRate));
            Assert.That(result.Qar.NationalAchievementRate, Is.EqualTo(providerSummaryModel.NationalAchievementRate));
            Assert.That(result.Reviews.ReviewPeriod, Is.EqualTo(providerSummaryModel.ReviewPeriod));
            Assert.That(result.Reviews.EmployerReviews, Is.EqualTo(providerSummaryModel.EmployerReviews));
            Assert.That(result.Reviews.EmployerStars, Is.EqualTo(providerSummaryModel.EmployerStars));
            Assert.That(result.Reviews.EmployerRating, Is.EqualTo(providerSummaryModel.EmployerRating));
            Assert.That(result.Reviews.ApprenticeReviews, Is.EqualTo(providerSummaryModel.ApprenticeReviews));
            Assert.That(result.Reviews.ApprenticeStars, Is.EqualTo(providerSummaryModel.ApprenticeStars));
            Assert.That(result.Reviews.ApprenticeRating, Is.EqualTo(providerSummaryModel.ApprenticeRating));
        });

        _providersRegistrationDetailReadRepositoryMock.Verify(a =>
            a.GetProviderSummary(query.Ukprn, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull(
        [Frozen] Mock<IProviderRegistrationDetailsReadRepository> _providersRegistrationDetailReadRepositoryMock,
        [Frozen] GetProviderSummaryQuery query,
        GetProviderSummaryQueryHandler sut,
        ProviderSummaryModel providerSummaryModel,
        CancellationToken cancellationToken
    )
    {
        _providersRegistrationDetailReadRepositoryMock.Setup(r =>
            r.GetProviderSummary(query.Ukprn, CancellationToken.None)
        ).ReturnsAsync((ProviderSummaryModel)null);

        var response = await sut.Handle(query, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Result, Is.Null);
        });

        _providersRegistrationDetailReadRepositoryMock.Verify(a =>
            a.GetProviderSummary(query.Ukprn, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}