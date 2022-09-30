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
        private readonly IProvidersReadRepository _providersReadRepository;


        public ReloadNationalAcheivementRatesService(
            ILogger<ReloadNationalAcheivementRatesService> logger,
            INationalAchievementRatesImportWriteRepository nationalAchievementRatesImportWriteRepository,
            INationalAchievementRatesImportReadRepository nationalAchievementRatesImportReadRepository,
            INationalAchievementRatesWriteRepository nationalAchievementRatesWriteRepository,
            IImportAuditWriteRepository importAuditWriteRepository,
            IProvidersReadRepository providersReadRepository)
        {
            _logger = logger;
            _nationalAchievementRatesImportWriteRepository = nationalAchievementRatesImportWriteRepository;
            _nationalAchievementRatesImportReadRepository = nationalAchievementRatesImportReadRepository;
            _nationalAchievementRatesWriteRepository = nationalAchievementRatesWriteRepository;
            _importAuditWriteRepository = importAuditWriteRepository;
            _providersReadRepository = providersReadRepository;
        }

        public async Task ReloadNationalAcheivementRates(List<NationalAchievementRatesApiModel> nationalAchievementRatesImported)
        {
            try
            {
                var timeStarted = DateTime.UtcNow;
                _logger.LogInformation("Clearing and Loading import table-NationalAchievementRatesImport");
                await _nationalAchievementRatesImportWriteRepository.Reload(nationalAchievementRatesImported.Select(c => (NationalAchievementRateImport)c).ToList());

                var nationalAchievementRatesImport = await _nationalAchievementRatesImportReadRepository.GetAllWithAchievementData();
                var allproviders = await _providersReadRepository.GetAllProviders();
                var nationalAchievementRates = new List<NationalAchievementRate>();
                foreach (var nationalAchievementRateImport in nationalAchievementRatesImport)
                {
                    var provider = allproviders.FirstOrDefault(p => p.Ukprn == nationalAchievementRateImport.Ukprn);
                    if(provider != null)
                    {
                        var nationalAchievementRate = (NationalAchievementRate)nationalAchievementRateImport;
                        nationalAchievementRate.ProviderId = provider.Id;
                        nationalAchievementRates.Add(nationalAchievementRate);
                    }
                }
                _logger.LogInformation("Clearing and Loading main table-NationalAchievementRate");

                await _nationalAchievementRatesWriteRepository.Reload(nationalAchievementRates); 

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
