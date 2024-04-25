using CsvHelper.Configuration.Attributes;

namespace SFA.DAS.Roatp.Jobs.Models;

public class ProviderAchievementRateCsvModel
{
    [Name("time_period")]
    public int TimePeriod { get; set; }
    [Name("std_fwk_name_stcode")]
    public string StandardFrameworkNameAndSTCode { get; set; }
    [Name("ssa_tier_1")]
    public string SectorSubjectAreaTier1Desc { get; set; }
    [Name("age_youth_adult")]
    public string AgeYouthAdult { get; set; }
    [Name("age_group")]
    public string AgeGroup { get; set; }
    [Name("provider_ukprn")]
    public int Ukprn { get; set; }
    [Name("level")]
    public string ApprenticeshipLevel { get; set; }
    [Name("leavers")]
    public string OverallCohort { get; set; }
    [Name("achievement_rate")]
    public string OverallAchievementRate { get; set; }
}
