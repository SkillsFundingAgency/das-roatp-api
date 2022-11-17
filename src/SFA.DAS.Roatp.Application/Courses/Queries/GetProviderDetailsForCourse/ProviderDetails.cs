using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class ProviderDetails
{
    public int Ukprn { get; set; }
    public string Name { get; set; }
    public string TradingName { get; set; }
    public string ContactUrl { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }

    public string StandardInfoUrl { get; set; }
    public string MarketingInfo { get; set; }
    public decimal? ProviderHeadOfficeDistanceInMiles { get; set; }

    public List<NationalAchievementRateModel> AchievementRates { get; set; } =
        new List<NationalAchievementRateModel>();

    public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

    public static implicit operator ProviderDetails(ProviderCourseDetailsModel providerCourseDetails)
    {
        if (providerCourseDetails == null)
            return null;

        var baseResult = (ProviderCourseDetailsModelBase)providerCourseDetails;
        
        var result = (ProviderDetails) baseResult;
        result.MarketingInfo = providerCourseDetails.MarketingInfo;
        result.ContactUrl = providerCourseDetails.StandardContactUrl;
        result.Email = providerCourseDetails.Email;
        result.Phone = providerCourseDetails.Phone;
        result.StandardInfoUrl = providerCourseDetails.StandardInfoUrl;
        result.Address1 = providerCourseDetails.Address1;
        result.Address2 = providerCourseDetails.Address2;
        result.Address3 = providerCourseDetails.Address3;
        result.Address4 = providerCourseDetails.Address4;
        result.Town = providerCourseDetails.Town;
        result.Postcode = providerCourseDetails.Postcode;
        return result;
    }

    public static implicit operator ProviderDetails(
        ProviderCourseDetailsModelBase providerCourseDetails)
    {

        return new ProviderDetails
        {
            Ukprn = providerCourseDetails.Ukprn,
            
            Name = providerCourseDetails.LegalName,
            TradingName = providerCourseDetails.TradingName,
            ProviderHeadOfficeDistanceInMiles = (decimal?)providerCourseDetails.Distance,
        };
    }
}