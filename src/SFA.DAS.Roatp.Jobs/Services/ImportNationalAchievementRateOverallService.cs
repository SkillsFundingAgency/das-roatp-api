using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Extensions;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ImportNationalAchievementRateOverallService : IImportNationalAchievementRateOverallService
{
    private readonly INationalAchievementRatesOverallImportWriteRepository _nationalAchievementRatesOverallImportWriteRepository;
    private readonly INationalAchievementRatesOverallWriteRepository _nationalAchievementRatesOverallWriteRepository;
    private readonly IImportAuditWriteRepository _importAuditWriteRepository;

    public ImportNationalAchievementRateOverallService(
        INationalAchievementRatesOverallImportWriteRepository nationalAchievementRatesOverallImportWriteRepository,
        INationalAchievementRatesOverallWriteRepository nationalAchievementRatesOverallWriteRepository,
        IImportAuditWriteRepository importAuditWriteRepository)
    {
        _nationalAchievementRatesOverallImportWriteRepository = nationalAchievementRatesOverallImportWriteRepository;
        _nationalAchievementRatesOverallWriteRepository = nationalAchievementRatesOverallWriteRepository;
        _importAuditWriteRepository = importAuditWriteRepository;
    }

    public async Task ImportData(IEnumerable<OverallAchievementRateCsvModel> rawData, List<SectorSubjectAreaTier1Model> ssa1s)
    {
        var timeStarted = DateTime.UtcNow;
        var overallImportData = rawData.Select(r => r.ConvertToEntity(ssa1s)).ToList();
        await _nationalAchievementRatesOverallImportWriteRepository.Reload(overallImportData);
        await _nationalAchievementRatesOverallWriteRepository.Reload(overallImportData.Select(o => (NationalAchievementRateOverall)o).ToList());
        await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, overallImportData.Count, ImportType.NationalAchievementRatesOverall));
    }
}
