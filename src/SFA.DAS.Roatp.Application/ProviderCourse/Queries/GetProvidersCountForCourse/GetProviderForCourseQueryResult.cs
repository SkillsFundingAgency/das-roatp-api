using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProvidersCountForCourse;

public class GetProviderForCourseQueryResult
{
    public int Ukprn { get; set; }

    public string Name { get; set; }
    public string TradingName { get; set; }
    public string ContactUrl { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Guid? ShortlistId { get; set; }
    public GetProviderSummaryAddress Address { get; set; }
    public string StandardInfoUrl { get; set; }
    public string MarketingInfo { get; set; }
    public GetProviderHeadOfficeAddress ProviderAddress { get; set; }
    public List<GetNationalAchievementRateResponse> AchievementRates { get; set; } = new List<GetNationalAchievementRateResponse>();
    public List<GetDeliveryTypesResponse> DeliveryTypes { get; set; } = new List<GetDeliveryTypesResponse>();

    // NOT REQUIRED
    // public GetEmployerFeedbackResponse EmployerFeedback { get; set; }

}

public class GetProviderSummaryAddress
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
}

public class GetProviderHeadOfficeAddress
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
    public double DistanceInMiles { get; set; }
}

public class GetNationalAchievementRateResponse
{
    public string Age { get; set; }

    public string Level { get; set; }

    public string SectorSubjectArea { get; set; }

    public decimal? OverallAchievementRate { get; set; }

    public int? OverallCohort { get; set; }

    public int Ukprn { get; set; }

}

public class GetDeliveryTypesResponse
{
    public decimal Radius { get; set; }

    public bool National { get; set; }

    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
    public string County { get; set; }

    public string DeliveryModes { get; set; }   // This is an amalgam?
    // EXAMPLES: 	100PercentEmployer|DayRelease|BlockRelease
    //              DayRelease
    //              100PercentEmployer|DayRelease

    public double DistanceInMiles { get; set; }

    public int LocationId { get; set; }
}