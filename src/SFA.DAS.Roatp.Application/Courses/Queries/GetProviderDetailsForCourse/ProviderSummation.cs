using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class ProviderSummation
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string TradingName { get; set; }
    public decimal? ProviderHeadOfficeDistanceInMiles { get; set; }
    public List<NationalAchievementRateModel> AchievementRates { get; set; } =
        new List<NationalAchievementRateModel>();

    public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

    public static implicit operator ProviderSummation(ProviderCourseDetailsSummaryModel providerDetails)
    {
        if (providerDetails == null)
            return null;

        return new ProviderSummation
        {
            Ukprn = providerDetails.Ukprn,
            Name = providerDetails.LegalName,
            TradingName = providerDetails.TradingName,
            ProviderHeadOfficeDistanceInMiles = (decimal?)providerDetails.Distance,
        };
    }
}