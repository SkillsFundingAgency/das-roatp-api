using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class ProviderSummation
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string TradingName { get; set; }
    public decimal? ProviderHeadOfficeDistanceInMiles { get; set; }
    public bool? IsApprovedByRegulator { get; set; }
    public List<NationalAchievementRateModel> AchievementRates { get; set; } =
        new List<NationalAchievementRateModel>();

    public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

    public static implicit operator ProviderSummation(ProviderCourseSummaryModel provider)
    {
        if (provider == null)
            return null;

        return new ProviderSummation
        {
            Ukprn = provider.Ukprn,
            Name = provider.LegalName,
            TradingName = provider.TradingName,
            ProviderHeadOfficeDistanceInMiles = (decimal?)provider.Distance,
            IsApprovedByRegulator = provider.IsApprovedByRegulator
        };
    }
}