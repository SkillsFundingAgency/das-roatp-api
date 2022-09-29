using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Data;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadNationalAcheivementRatesService : IReloadNationalAcheivementRatesService
    {
        private readonly ILogger<ReloadNationalAcheivementRatesService> _logger;
        private readonly INationalAchievementRatesImportWriteRepository _nationalAchievementRatesImportWriteRepository;
        private readonly INationalAchievementRatesImportReadRepository _nationalAchievementRatesImportReadRepository;
        private readonly INationalAchievementRatesWriteRepository _nationalAchievementRatesWriteRepository;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;

        public ReloadNationalAcheivementRatesService(
            ILogger<ReloadNationalAcheivementRatesService> logger,
            INationalAchievementRatesImportWriteRepository nationalAchievementRatesImportWriteRepository,
            INationalAchievementRatesImportReadRepository nationalAchievementRatesImportReadRepository,
            INationalAchievementRatesWriteRepository nationalAchievementRatesWriteRepository,
            IImportAuditWriteRepository importAuditWriteRepository
            )
        {
            _logger = logger;
            _nationalAchievementRatesImportWriteRepository = nationalAchievementRatesImportWriteRepository;
            _nationalAchievementRatesImportReadRepository = nationalAchievementRatesImportReadRepository;
            _nationalAchievementRatesWriteRepository = nationalAchievementRatesWriteRepository;
            _importAuditWriteRepository = importAuditWriteRepository;
        }

        public async Task ReloadNationalAcheivementRates(List<NationalAchievementRatesApiModel> nationalAchievementRatesImported)
        {
            try
            {
                var timeStarted = DateTime.UtcNow;
                _logger.LogInformation("Clearing and Loading import table-NationalAchievementRatesImport");
                await _nationalAchievementRatesImportWriteRepository.Reload(nationalAchievementRatesImported.Select(c => (NationalAchievementRateImport)c).ToList());

                var nationalAchievementRates = await _nationalAchievementRatesImportReadRepository.GetAllWithAchievementData();
                _logger.LogInformation("Clearing and Loading main table-NationalAchievementRate");
                await _nationalAchievementRatesWriteRepository.Reload(nationalAchievementRates.Select(c => (NationalAchievementRate)c).ToList());

                _logger.LogInformation($"Loaded  {nationalAchievementRates.Count} National Achievement Rates");
                await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, nationalAchievementRates.Count, ImportType.NationalAchievementRates));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reloading the NationalAchievementRate data");
            }
        }
    }
}
