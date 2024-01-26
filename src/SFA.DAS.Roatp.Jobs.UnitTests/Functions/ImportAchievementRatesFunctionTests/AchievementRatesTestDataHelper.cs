using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions.ImportAchievementRatesFunctionTests;

public static class AchievementRatesTestDataHelper
{
    public static class ApprenticeshipLevel
    {
        public const string Total = nameof(Total);
        public const string Advanced = "Advanced Level";
        public const string Intermediate = "Intermediate Level";
        public const string Higher = "Higher Level";
    }
    public const string Ssa1WithValidData = "Construction, Planning and the Built Environment";
    public const string Ssa1WithInvalidData = "Engineering and Manufacturing Technologies";

    public const string Total = ImportAchievementRatesFunction.Total;

    public static List<OverallAchievementRateCsvModel> GetValidOverallRawData()
        => new()
        {
            GetOverallAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Total, "10", "88.5"),
            GetOverallAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Intermediate, "10", "88.5"),
            GetOverallAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Advanced, "10", "88.5"),
            GetOverallAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"),
        };

    public static List<OverallAchievementRateCsvModel> GetIgnoredOverallRawData()
        => new()
        {
            GetOverallAchievementRateCsvModel("Framework", Ssa1WithInvalidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Total, "10", "88.5"), // frameworkStandardFlag
            GetOverallAchievementRateCsvModel("Standard", Ssa1WithInvalidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Intermediate, "10", "88.5"), //frameworkStandardFlag
            GetOverallAchievementRateCsvModel(Total, Total, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Advanced, "10", "88.5"), // SSA1 
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, "Accounting and Finance", Total, Total, Total, Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // SSA2
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, "Actuarial Technician-ST0004", Total, Total, Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // StandardFrameworkNameAndSTCode
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, "Level 7", Total, Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // DetailedLevel
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, "Level 6", Total, Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // DetailedLevel
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, "16-19", Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // AgeYouthAdult
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, "19+", Total, Total, ApprenticeshipLevel.Higher, "10", "88.5"), // AgeYouthAdult
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, "Other", Total, ApprenticeshipLevel.Higher, "10", "88.5"), // FundingType
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, "Supported by ASA levy funds", Total, ApprenticeshipLevel.Higher, "10", "88.5"), // FundingType
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, "16-18", ApprenticeshipLevel.Higher, "10", "88.5"), // AgeGroup
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, "19-23", ApprenticeshipLevel.Higher, "10", "88.5"), // AgeGroup
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, "24+", ApprenticeshipLevel.Higher, "10", "88.5"), // AgeGroup
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Higher, "low", "88.5"), // OverallCohort
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Higher, "z", "88.5"), // OverallCohort
            GetOverallAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, Total, Total, Total, Total, ApprenticeshipLevel.Higher, "10", "x"), // AchievementRate
        };

    public static List<OverallAchievementRateCsvModel> GetAllOverallRatingsRawData()
    => new(GetValidOverallRawData().Concat(GetIgnoredOverallRawData()));

    public static List<ProviderAchievementRateCsvModel> GetValidProviderRatingsRawData()
        => new()
        {
            GetProviderAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, 10046498, ApprenticeshipLevel.Total, "230", "47.6"),
            GetProviderAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, 10046498, ApprenticeshipLevel.Intermediate, "230", "47.6"),
            GetProviderAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, 10046498, ApprenticeshipLevel.Higher, "230", "47.6"),
            GetProviderAchievementRateCsvModel(Total, Ssa1WithValidData, Total, Total, 10046498, ApprenticeshipLevel.Advanced, "230", "47.6"),
        };

    public static List<ProviderAchievementRateCsvModel> GetIgnoredProviderRatingsRawData()
        => new()
        {
            GetProviderAchievementRateCsvModel("Construction Plant Operative-ST0736", Ssa1WithInvalidData, Total, Total, 10046498, ApprenticeshipLevel.Total, "230", "47.6"), // StandardFrameworkNameAndSTCode
            GetProviderAchievementRateCsvModel(Total, Total, Total, Total, 10046498, ApprenticeshipLevel.Intermediate, "230", "47.6"), // SSA1
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, "16-18", Total, 10046498, ApprenticeshipLevel.Higher, "230", "47.6"), // AgeYouthGroup
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, "19+", Total, 10046498, ApprenticeshipLevel.Higher, "230", "47.6"), // AgeYouthGroup
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, "16-18", 10046498, ApprenticeshipLevel.Advanced, "230", "47.6"), // AgeGroup
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, "19-23", 10046498, ApprenticeshipLevel.Advanced, "230", "47.6"), // AgeGroup
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, "24+", 10046498, ApprenticeshipLevel.Advanced, "230", "47.6"), // AgeGroup
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, 10046498, ApprenticeshipLevel.Advanced, "low", "x"), // OverallCohort
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, 10046498, ApprenticeshipLevel.Advanced, "c", "x"), // OverallCohort
            GetProviderAchievementRateCsvModel(Total, Ssa1WithInvalidData, Total, Total, 10046498, ApprenticeshipLevel.Advanced, "10", "x"), // AchievementRate
        };

    public static List<ProviderAchievementRateCsvModel> GetAllProviderRatingsRawData() => new(GetValidProviderRatingsRawData().Concat(GetIgnoredProviderRatingsRawData()));
    private static OverallAchievementRateCsvModel GetOverallAchievementRateCsvModel(
        string frameworkStandardFlag,
        string sectorSubjectAreaTier1,
        string sectorSubjectAreaTier2,
        string standardFrameworkNameAndSTCode,
        string detailedLevel,
        string ageYouthAdult,
        string fundingType,
        string ageGroup,
        string apprenticeshipLevel,
        string overallCohort,
        string overallAchievementRate
    ) => new()
    {
        FrameworkStandardFlag = frameworkStandardFlag,
        SectorSubjectAreaTier1Desc = sectorSubjectAreaTier1,
        SectorSubjectAreaTier2Desc = sectorSubjectAreaTier2,
        StandardFrameworkNameAndSTCode = standardFrameworkNameAndSTCode,
        DetailedLevel = detailedLevel,
        AgeYouthAdult = ageYouthAdult,
        FundingType = fundingType,
        AgeGroup = ageGroup,
        ApprenticeshipLevel = apprenticeshipLevel,
        OverallCohort = overallCohort,
        OverallAchievementRate = overallAchievementRate
    };

    private static ProviderAchievementRateCsvModel GetProviderAchievementRateCsvModel(
        string standardFrameworkNameAndSTCode,
        string sectorSubjectAreaTier1,
        string ageYouthAdult,
        string ageGroup,
        int ukprn,
        string apprenticeshipLevel,
        string overallCohort,
        string overallAchievementRate
    ) => new()
    {
        StandardFrameworkNameAndSTCode = standardFrameworkNameAndSTCode,
        SectorSubjectAreaTier1Desc = sectorSubjectAreaTier1,
        AgeYouthAdult = ageYouthAdult,
        AgeGroup = ageGroup,
        Ukprn = ukprn,
        ApprenticeshipLevel = apprenticeshipLevel,
        OverallCohort = overallCohort,
        OverallAchievementRate = overallAchievementRate
    };
}
