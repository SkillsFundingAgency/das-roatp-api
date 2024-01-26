using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Models;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class ImportAchievementRatesFunction
{
    public const string ProviderRatingsImportFileName = "apps_narts_provider_level_fwk_std.csv";
    public const string OverallRatingsImportFileName = "apps_narts_subject_and_level_detailed.csv";
    public const string Total = nameof(Total);

    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IImportNationalAchievementRateOverallService _importAchievementRateOverallService;
    private readonly IImportNationalAchievementRateService _importNationalAchievementRateService;

    public ImportAchievementRatesFunction(IDataExtractorService dataExtractorService, ICourseManagementOuterApiClient courseManagementOuterApiClient, IImportNationalAchievementRateOverallService importAchievementRateOverallService, IImportNationalAchievementRateService importNationalAchievementRateService)
    {
        _dataExtractorService = dataExtractorService;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _importAchievementRateOverallService = importAchievementRateOverallService;
        _importNationalAchievementRateService = importNationalAchievementRateService;
    }

    [FunctionName("Achievement-Rates-Import")]
    public async Task Run([BlobTrigger("qar-updates/{name}")] Stream blobStream, string name, ILogger log)
    {
        log.LogInformation($"Beginning to process blob\n Name:{name}\n Size:{blobStream.Length} bytes");

        var ssa1s = await GetStandardSectorAreaTier1LookupData();

        var rawOverallData = _dataExtractorService.DeserializeCsvDataFromZipStream<OverallAchievementRateCsvModel>(blobStream, OverallRatingsImportFileName);
        log.LogInformation($"Overall achievement rates import data total row count: {rawOverallData.Count}");

        var filteredOverallRatingsData = rawOverallData.Where(o => o.FrameworkStandardFlag == Total && o.SectorSubjectAreaTier1Desc != Total && o.SectorSubjectAreaTier2Desc == Total && o.StandardFrameworkNameAndSTCode == Total && o.DetailedLevel == Total && o.AgeYouthAdult == Total && o.AgeGroup == Total && o.FundingType == Total && int.TryParse(o.OverallCohort, out _) && decimal.TryParse(o.OverallAchievementRate, out _));

        if (rawOverallData.Any()) await _importAchievementRateOverallService.ImportData(filteredOverallRatingsData, ssa1s);

        var rawProviderData = _dataExtractorService.DeserializeCsvDataFromZipStream<ProviderAchievementRateCsvModel>(blobStream, ProviderRatingsImportFileName);
        log.LogInformation($"Provider achievement rates import data total row count: {rawProviderData.Count}");

        var filteredProviderRatingsData = rawProviderData.Where(p => p.StandardFrameworkNameAndSTCode == Total && p.SectorSubjectAreaTier1Desc != Total && p.AgeYouthAdult == Total && p.AgeGroup == Total && int.TryParse(p.OverallCohort, out _) && decimal.TryParse(p.OverallAchievementRate, out _));

        if (rawProviderData.Any()) await _importNationalAchievementRateService.ImportData(filteredProviderRatingsData, ssa1s);
    }

    private async Task<List<SectorSubjectAreaTier1Model>> GetStandardSectorAreaTier1LookupData()
    {
        var (success, getAllSectorSubjectAreaTier1Response) = await _courseManagementOuterApiClient.Get<GetAllSectorSubjectAreaTier1Response>("lookup/sector-subject-area-tier1");
        if (!success)
        {
            throw new InvalidOperationException("Unexpected response when trying to get sector subject area tier1 lookup data from the course management outer api.");
        }
        return getAllSectorSubjectAreaTier1Response.SectorSubjectAreaTier1s;
    }
}
