using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Models;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class ImportAchievementRatesFunction
{
    public const string Total = nameof(Total);

    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly IDataExtractorService _dataExtractorService;
    private readonly IImportNationalAchievementRateOverallService _importAchievementRateOverallService;
    private readonly IImportNationalAchievementRateService _importNationalAchievementRateService;
    private readonly string _timePeriod;
    private readonly string _providerRatingsImportFileName;
    private readonly string _overallRatingsImportFileName;

    public ImportAchievementRatesFunction(IDataExtractorService dataExtractorService, ICourseManagementOuterApiClient courseManagementOuterApiClient, IImportNationalAchievementRateOverallService importAchievementRateOverallService, IImportNationalAchievementRateService importNationalAchievementRateService, IConfiguration configuration)
    {
        _dataExtractorService = dataExtractorService;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _importAchievementRateOverallService = importAchievementRateOverallService;
        _importNationalAchievementRateService = importNationalAchievementRateService;

        _timePeriod = configuration["QarTimePeriod"];
        _providerRatingsImportFileName = configuration["QarProviderLevelImportFileName"];
        _overallRatingsImportFileName = configuration["QarOverallImportFileName"];
    }

    [Function("Achievement-Rates-Import")]
    public async Task Run([BlobTrigger("qar-updates/{name}")] Stream blobStream, string name, ILogger log)
    {
        log.LogInformation("Beginning to process blob\n Name:{FileName}\n Size:{BlobStreamLength} bytes", name, blobStream.Length);

        if (string.IsNullOrWhiteSpace(_timePeriod) || !int.TryParse(_timePeriod, out var timePeriod))
        {
            throw new ArgumentException("QarTimePeriod is either not set in environment value or is invalid. Expected a number ex. 202223");
        }

        var ssa1s = await GetStandardSectorAreaTier1LookupData();

        var rawOverallData = _dataExtractorService.DeserializeCsvDataFromZipStream<OverallAchievementRateCsvModel>(blobStream, _overallRatingsImportFileName);
        log.LogInformation("Overall achievement rates import data total row count: {QarImportOverallCount}", rawOverallData.Count);

        var filteredOverallRatingsData = rawOverallData.Where(o =>
               o.TimePeriod == timePeriod
            && o.FrameworkStandardFlag == Total
            && o.SectorSubjectAreaTier1Desc != Total
            && o.SectorSubjectAreaTier2Desc == Total
            && o.StandardFrameworkNameAndSTCode == Total
            && o.DetailedLevel == Total
            && o.AgeYouthAdult == Total
            && o.AgeGroup == Total
            && o.FundingType == Total
            && int.TryParse(o.OverallCohort, out _)
            && decimal.TryParse(o.OverallAchievementRate, out _));

        if (rawOverallData.Count > 0) await _importAchievementRateOverallService.ImportData(filteredOverallRatingsData, ssa1s);

        var rawProviderData = _dataExtractorService.DeserializeCsvDataFromZipStream<ProviderAchievementRateCsvModel>(blobStream, _providerRatingsImportFileName);
        log.LogInformation("Provider achievement rates import data total row count: {QarImportProviderLevelCount}", rawProviderData.Count);

        var filteredProviderRatingsData = rawProviderData.Where(p =>
               p.TimePeriod == timePeriod
            && p.StandardFrameworkNameAndSTCode == Total
            && p.SectorSubjectAreaTier1Desc != Total
            && p.AgeYouthAdult == Total
            && p.AgeGroup == Total
            && int.TryParse(p.OverallCohort, out _)
            && decimal.TryParse(p.OverallAchievementRate, out _));

        if (rawProviderData.Count > 0) await _importNationalAchievementRateService.ImportData(filteredProviderRatingsData, ssa1s);
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
