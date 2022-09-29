using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadNationalAcheivementRatesOverallService : IReloadNationalAcheivementRatesOverallService
    {
        private readonly ILogger<ReloadNationalAcheivementRatesOverallService> _logger;
        private readonly INationalAchievementRatesOverallImportWriteRepository _nationalAchievementRatesOverallImportWriteRepository;
        private readonly INationalAchievementRatesOverallImportReadRepository _nationalAchievementRatesOverallImportReadRepository;
        private readonly INationalAchievementRatesOverallWriteRepository _nationalAchievementRatesOverallWriteRepository;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;

        public ReloadNationalAcheivementRatesOverallService(
            ILogger<ReloadNationalAcheivementRatesOverallService> logger,
            INationalAchievementRatesOverallImportWriteRepository nationalAchievementRatesOverallImportWriteRepository,
            INationalAchievementRatesOverallImportReadRepository nationalAchievementRatesOverallImportReadRepository,
            INationalAchievementRatesOverallWriteRepository nationalAchievementRatesOverallWriteRepository,
            IImportAuditWriteRepository importAuditWriteRepository
            )
        {
            _logger = logger;
            _nationalAchievementRatesOverallImportWriteRepository = nationalAchievementRatesOverallImportWriteRepository;
            _nationalAchievementRatesOverallImportReadRepository = nationalAchievementRatesOverallImportReadRepository;
            _nationalAchievementRatesOverallWriteRepository = nationalAchievementRatesOverallWriteRepository;
            _importAuditWriteRepository = importAuditWriteRepository;
        }

        public async Task ReloadNationalAcheivementRatesOverall(List<NationalAchievementRatesOverallApiModel> OverallAchievementRatesImported)
        {
            try
            {
                var timeStarted = DateTime.UtcNow;
                _logger.LogInformation("Clearing and Loading import table-NationalAchievementRatesOverallImport");
                await _nationalAchievementRatesOverallImportWriteRepository.Reload(OverallAchievementRatesImported.Select(c => (NationalAchievementRateOverallImport)c).ToList());

                var nationalAchievementRatesOverall = await _nationalAchievementRatesOverallImportReadRepository.GetAllWithAchievementData();
                _logger.LogInformation("Clearing and Loading main table-NationalAchievementRateOverall");
                await _nationalAchievementRatesOverallWriteRepository.Reload(nationalAchievementRatesOverall.Select(c => (NationalAchievementRateOverall)c).ToList());

                _logger.LogInformation($"Loaded  {nationalAchievementRatesOverall.Count} National Achievement Rates Overall");
                await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, nationalAchievementRatesOverall.Count, ImportType.NationalAchievementRatesOverall));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reloading the NationalAchievementRateOverall data");
            }
        }
    }
}
