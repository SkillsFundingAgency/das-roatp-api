using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;

public class GetProviderSummaryQueryResult
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string TradingName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ContactUrl { get; set; }
    public int ProviderTypeId { get; set; }
    public int StatusId { get; set; }
    public string MarketingInfo { get; set; }
    public bool CanAccessApprenticeshipService { get; set; }
    public ProviderAddressModel Address { get; set; }
    public ProviderQarModel Qar { get; set; }
    public ReviewsModel Reviews { get; set; }

    public static implicit operator GetProviderSummaryQueryResult(ProviderSummaryModel source)
    {
        return new GetProviderSummaryQueryResult
        {
            Ukprn = source.Ukprn,
            Name = source.LegalName,
            TradingName = source.TradingName,
            Email = source.Email,
            Phone = source.Phone,
            ContactUrl = source.ContactUrl,
            ProviderTypeId = source.ProviderTypeId,
            StatusId = source.StatusId,
            MarketingInfo = source.MarketingInfo,
            CanAccessApprenticeshipService = source.CanAccessApprenticeshipService,
            Address = new ProviderAddressModel()
            {
                AddressLine1 = source.MainAddressLine1,
                AddressLine2 = source.MainAddressLine2,
                AddressLine3 = source.MainAddressLine3,
                AddressLine4 = source.MainAddressLine4,
                Town = source.MainTown,
                Postcode = source.MainPostcode,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
            },
            Qar = new ProviderQarModel() {
                Period = source.QARPeriod,
                Leavers = source.Leavers,
                AchievementRate = source.AchievementRate,
                NationalAchievementRate = source.NationalAchievementRate
            },
            Reviews = new ReviewsModel()
            {
                ReviewPeriod = source.ReviewPeriod,
                EmployerReviews = source.EmployerReviews,
                EmployerStars = source.EmployerStars,
                EmployerRating = source.EmployerRating,
                ApprenticeReviews = source.ApprenticeReviews,
                ApprenticeStars = source.ApprenticeStars,
                ApprenticeRating = source.ApprenticeRating
            }
        };
    }
}
