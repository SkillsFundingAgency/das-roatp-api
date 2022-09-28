using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadNationalAcheivementRatesService : IReloadNationalAcheivementRatesService
    {
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<ReloadNationalAcheivementRatesService> _logger;
        private readonly INationalAchievementRatesImportWriteRepository _nationalAchievementRatesImportWriteRepository;
        private readonly INationalAchievementRatesImportReadRepository _nationalAchievementRatesImportReadRepository;
        private readonly INationalAchievementRatesWriteRepository _nationalAchievementRatesWriteRepository;
        private readonly INationalAchievementRatesOverallImportWriteRepository _nationalAchievementRatesOverallImportWriteRepository;
        private readonly INationalAchievementRatesOverallImportReadRepository _nationalAchievementRatesOverallImportReadRepository;
        private readonly INationalAchievementRatesOverallWriteRepository _nationalAchievementRatesOverallWriteRepository;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;

        public ReloadNationalAcheivementRatesService(ICourseManagementOuterApiClient courseManagementOuterApiClient,
            ILogger<ReloadNationalAcheivementRatesService> logger,
            INationalAchievementRatesImportWriteRepository nationalAchievementRatesImportWriteRepository,
            INationalAchievementRatesImportReadRepository nationalAchievementRatesImportReadRepository,
            INationalAchievementRatesWriteRepository nationalAchievementRatesWriteRepository,
            INationalAchievementRatesOverallImportWriteRepository nationalAchievementRatesOverallImportWriteRepository,
            INationalAchievementRatesOverallImportReadRepository nationalAchievementRatesOverallImportReadRepository,
            INationalAchievementRatesOverallWriteRepository nationalAchievementRatesOverallWriteRepository,
            IImportAuditWriteRepository importAuditWriteRepository
            )
        {
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _logger = logger;
            _nationalAchievementRatesImportWriteRepository = nationalAchievementRatesImportWriteRepository;
            _nationalAchievementRatesImportReadRepository = nationalAchievementRatesImportReadRepository;
            _nationalAchievementRatesWriteRepository = nationalAchievementRatesWriteRepository;
            _nationalAchievementRatesOverallImportWriteRepository = nationalAchievementRatesOverallImportWriteRepository;
            _nationalAchievementRatesOverallImportReadRepository = nationalAchievementRatesOverallImportReadRepository;
            _nationalAchievementRatesOverallWriteRepository = nationalAchievementRatesOverallWriteRepository;
            _importAuditWriteRepository = importAuditWriteRepository;
        }

        public async Task ReloadNationalAcheivementRates()
        {
            var timeStarted = DateTime.UtcNow;
            var (success, nationalAchievementRatesLookup) = await _courseManagementOuterApiClient.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates");
            if (!success)
            {
                const string errorMessage = "Unexpected response when trying to get national achievement rates from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            _logger.LogInformation("Clearing import table-NationalAchievementRatesImport");
            await _nationalAchievementRatesImportWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to import table-NationalAchievementRatesImport");
            await _nationalAchievementRatesImportWriteRepository.InsertMany(nationalAchievementRatesLookup.NationalAchievementRates.Select(c => (NationalAchievementRateImport)c).ToList());

            _logger.LogInformation("Clearing main table-NationalAchievementRate");
            await _nationalAchievementRatesWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to main table-NationalAchievementRate");
            var nationalAchievementRates = await _nationalAchievementRatesImportReadRepository.GetAllWithAchievementData();
            await _nationalAchievementRatesWriteRepository.InsertMany(nationalAchievementRates.Select(c => (NationalAchievementRate)c).ToList());
            
            _logger.LogInformation($"Loaded  {nationalAchievementRates.Count} National Achievement Rates");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, nationalAchievementRates.Count, ImportType.NationalAchievementRates));

            _logger.LogInformation("Clearing import table-NationalAchievementRatesOverallImport");
            await _nationalAchievementRatesOverallImportWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to import table-NationalAchievementRatesOverallImport");
            await _nationalAchievementRatesOverallImportWriteRepository.InsertMany(nationalAchievementRatesLookup.OverallAchievementRates.Select(c => (NationalAchievementRateOverallImport)c).ToList());

            _logger.LogInformation("Clearing main table-NationalAchievementRateOverall");
            await _nationalAchievementRatesOverallWriteRepository.DeleteAll();

            _logger.LogInformation("Loading to main table-NationalAchievementRatesOverall");
            var nationalAchievementRatesOverall = await _nationalAchievementRatesOverallImportReadRepository.GetAllWithAchievementData();
            await _nationalAchievementRatesOverallWriteRepository.InsertMany(nationalAchievementRatesOverall.Select(c => (NationalAchievementRateOverall)c).ToList());
            
            _logger.LogInformation($"Loaded  {nationalAchievementRatesOverall.Count} National Achievement Rates Overall");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, nationalAchievementRatesOverall.Count, ImportType.NationalAchievementRatesOverall));
        }
    }
}
