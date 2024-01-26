using CsvHelper.Configuration.Attributes;

namespace SFA.DAS.Roatp.Jobs.Models;

public class OverallAchievementRateCsvModel
{
    [Name("fwk_std_flag")]
    public string FrameworkStandardFlag { get; set; }
    [Name("ssa1")]
    public string SectorSubjectAreaTier1Desc { get; set; }
    [Name("ssa2")]
    public string SectorSubjectAreaTier2Desc { get; set; }
    [Name("std_fwk_name_stcode")]
    public string StandardFrameworkNameAndSTCode { get; set; }
    [Name("detailed_level")]
    public string DetailedLevel { get; set; }
    [Name("age_youth_adult")]
    public string AgeYouthAdult { get; set; }
    [Name("funding_type")]
    public string FundingType { get; set; }
    [Name("age_group")]
    public string AgeGroup { get; set; }
    [Name("level")]
    public string ApprenticeshipLevel { get; set; }
    [Name("leavers")]
    public string OverallCohort { get; set; }
    [Name("achievement_rate")]
    public string OverallAchievementRate { get; set; }
}
