using System.Collections.Generic;
using System.Linq;
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
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? ProviderHeadOfficeDistanceInMiles { get; set; }

    //MFCMFC
    // public decimal? ShortestLocationDistanceInMiles {
    //     get
    //     {
    //         if (DeliveryModels is null || !DeliveryModels.Any())
    //             return null;
    //         
    //         return DeliveryModels.Where(d=>d.DeliveryModeType is DeliveryModeType.DayRelease or DeliveryModeType.BlockRelease).Select(x => (decimal?)x.DistanceInMiles).MinBy(x=> x);
    //     }
    // }

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

        return result;

    }


    public static implicit operator ProviderDetails(
        ProviderCourseDetailsSummaryModel providerCourseDetails)
    {
        if (providerCourseDetails == null)
            return null;

        return (ProviderCourseDetailsModelBase)providerCourseDetails;
    }


    public static implicit operator ProviderDetails(
        ProviderCourseDetailsModelBase providerCourseDetails)
    {

        return new ProviderDetails
        {
            Ukprn = providerCourseDetails.Ukprn,
            ContactUrl = providerCourseDetails.StandardContactUrl,
            Email = providerCourseDetails.Email,
            Phone = providerCourseDetails.Phone,
            Name = providerCourseDetails.LegalName,
            TradingName = providerCourseDetails.TradingName,
            StandardInfoUrl = providerCourseDetails.StandardInfoUrl,
            Address1 = providerCourseDetails.Address1,
            Address2 = providerCourseDetails.Address2,
            Address3 = providerCourseDetails.Address3,
            Address4 = providerCourseDetails.Address4,
            Town = providerCourseDetails.Town,
            Postcode = providerCourseDetails.Postcode,
            ProviderHeadOfficeDistanceInMiles = (decimal?)providerCourseDetails.Distance,
        };
    }
}