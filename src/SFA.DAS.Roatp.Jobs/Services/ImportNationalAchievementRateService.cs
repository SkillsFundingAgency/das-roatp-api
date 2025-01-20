using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Extensions;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ImportNationalAchievementRateService : IImportNationalAchievementRateService
{
    private readonly INationalAchievementRatesImportWriteRepository _nationalAchievementRatesImportWriteRepository;
    private readonly INationalAchievementRatesWriteRepository _nationalAchievementRatesWriteRepository;
    private readonly IImportAuditWriteRepository _importAuditWriteRepository;

    public ImportNationalAchievementRateService(INationalAchievementRatesImportWriteRepository nationalAchievementRatesImportWriteRepository, INationalAchievementRatesWriteRepository nationalAchievementRatesWriteRepository, IImportAuditWriteRepository importAuditWriteRepository)
    {
        _nationalAchievementRatesImportWriteRepository = nationalAchievementRatesImportWriteRepository;
        _nationalAchievementRatesWriteRepository = nationalAchievementRatesWriteRepository;
        _importAuditWriteRepository = importAuditWriteRepository;
    }

    public async Task ImportData(IEnumerable<ProviderAchievementRateCsvModel> rawData, List<SectorSubjectAreaTier1Model> ssa1s)
    {
        var timeStarted = DateTime.UtcNow;
        var providerImportData = rawData.Select(r => r.ConvertToEntity(ssa1s)).ToList();
        await _nationalAchievementRatesImportWriteRepository.Reload(providerImportData);
        await _nationalAchievementRatesWriteRepository.Reload(providerImportData.Select(o => (NationalAchievementRate)o).ToList());
        await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerImportData.Count, ImportType.NationalAchievementRates));
    }
}
