using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Courses.GetProviderDetailsForCourse
{
    public class GetProviderDetailsForCourseQueryResult
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
        public double DistanceInMiles { get; set; }

        public string StandardInfoUrl { get; set; }    //MFCMFC this is course information
        public string MarketingInfo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }     
        public List<NationalAchievementRateModel> AchievementRates { get; set; } =
            new List<NationalAchievementRateModel>();

        public List<GetDeliveryTypesResponse> DeliveryTypes { get; set; } = new List<GetDeliveryTypesResponse>();

        // NOT REQUIRED
        // public GetEmployerFeedbackResponse EmployerFeedback { get; set; }


        public static implicit operator GetProviderDetailsForCourseQueryResult(ProviderDetailsWithDistance providerDetails) =>
            new()
            {
                Ukprn = providerDetails.Ukprn,
                Email = providerDetails.Email,
                Phone = providerDetails.Phone,
                Name = providerDetails.LegalName,
                TradingName = providerDetails.TradingName,
                MarketingInfo = providerDetails.MarketingInfo,
                Address1 = providerDetails.AddressLine1,
                Address2 = providerDetails.AddressLine2,
                Address3 = providerDetails.AddressLine3,
                Address4 = providerDetails.AddressLine4,
                Town = providerDetails.Town,
                Postcode = providerDetails.Postcode,
                DistanceInMiles = providerDetails.Distance,
                Latitude = providerDetails.Latitude,
                Longitude = providerDetails.Longitude,
            };

    }

    public class GetDeliveryTypesResponse
    {
        public bool National { get; set; }

        // convert to subclass and include lat/long, distance
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public double DistanceInMiles { get; set; }


        public string DeliveryModes { get; set; } // This is an amalgam?
        // EXAMPLES: 	100PercentEmployer|DayRelease|BlockRelease
        //              DayRelease
        //              100PercentEmployer|DayRelease

      // blockRelease bool
      // blockrelease distance
      // dayRelease bool
      // day release distance
      // regionname
      // subregionname
      // locationname
      // LocationType

      public int LocationId { get; set; }
    }
}