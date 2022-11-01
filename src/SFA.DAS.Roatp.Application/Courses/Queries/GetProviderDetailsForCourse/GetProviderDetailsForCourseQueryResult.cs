﻿using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse
{
    public class GetProviderDetailsForCourseQueryResult
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }

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
        public double Distance { get; set; }
        public string DeliveryModes
        {
            get
            {
                var deliveryModes = new List<string>();
                if (LocationDetails.Any(x => x.BlockRelease))
                    deliveryModes.Add("BlockRelease");

                if (LocationDetails.Any(x => x.DayRelease))
                    deliveryModes.Add("DayRelease");

                if (LocationDetails.Any(x => x.LocationType == LocationType.Regional)
                    || LocationDetails.Any(x => x.LocationType == LocationType.National))
                    deliveryModes.Add("100PercentEmployer");

                return string.Join("|", deliveryModes);
            }
        }
        public List<NationalAchievementRateModel> AchievementRates { get; set; } =
            new List<NationalAchievementRateModel>();

        public List<LocationDetail> LocationDetails { get; set; } = new List<LocationDetail>();

        public static implicit operator GetProviderDetailsForCourseQueryResult(ProviderAndCourseDetailsWithDistance providerAndCourseDetails)
        {
            if (providerAndCourseDetails == null)
                return null;

            return new GetProviderDetailsForCourseQueryResult
            {
                Ukprn = providerAndCourseDetails.Ukprn,
                LarsCode = providerAndCourseDetails.LarsCode,
                ContactUrl = providerAndCourseDetails.StandardContactUrl,
                Email = providerAndCourseDetails.Email,
                Phone = providerAndCourseDetails.Phone,
                Name = providerAndCourseDetails.LegalName,
                TradingName = providerAndCourseDetails.TradingName,
                MarketingInfo = providerAndCourseDetails.MarketingInfo,
                StandardInfoUrl = providerAndCourseDetails.StandardInfoUrl,
                Address1 = providerAndCourseDetails.Address1,
                Address2 = providerAndCourseDetails.Address2,
                Address3 = providerAndCourseDetails.Address3,
                Address4 = providerAndCourseDetails.Address4,
                Town = providerAndCourseDetails.Town,
                Postcode = providerAndCourseDetails.Postcode,
                Distance = providerAndCourseDetails.Distance,
                Latitude = providerAndCourseDetails.Latitude,
                Longitude = providerAndCourseDetails.Longitude
            };
        }
    }
}