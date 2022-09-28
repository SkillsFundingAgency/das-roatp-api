using Microsoft.Extensions.Logging;
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

        public async Task ReloadNationalAcheivementRates(List<NationalAchievementRatesApiImport> nationalAchievementRatesImported)
        {
            var timeStarted = DateTime.UtcNow;
            _logger.LogInformation("Clearing import table-NationalAchievementRatesImport");
            await _nationalAchievementRatesImportWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to import table-NationalAchievementRatesImport");
            await _nationalAchievementRatesImportWriteRepository.InsertMany(nationalAchievementRatesImported.Select(c => (NationalAchievementRateImport)c).ToList());

            _logger.LogInformation("Clearing main table-NationalAchievementRate");
            await _nationalAchievementRatesWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to main table-NationalAchievementRate");
            var nationalAchievementRates = await _nationalAchievementRatesImportReadRepository.GetAllWithAchievementData();
            await _nationalAchievementRatesWriteRepository.InsertMany(nationalAchievementRates.Select(c => (NationalAchievementRate)c).ToList());
            
            _logger.LogInformation($"Loaded  {nationalAchievementRates.Count} National Achievement Rates");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, nationalAchievementRates.Count, ImportType.NationalAchievementRates));
        }
    }
}
