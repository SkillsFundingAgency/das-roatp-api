using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadStandardsCacheService : IReloadStandardsCacheService
    {
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly IReloadStandardsRepository _reloadStandardsRepository;
        private readonly IImportAuditInsertRepository _importAuditInsertRepository;
        private readonly ILogger<ReloadStandardsCacheService> _logger;

        public ReloadStandardsCacheService(ILogger<ReloadStandardsCacheService> logger, ICourseManagementOuterApiClient courseManagementOuterApiClient, IReloadStandardsRepository reloadStandardsRepository, IImportAuditInsertRepository importAuditInsertRepository)
        {
            _logger = logger;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _reloadStandardsRepository = reloadStandardsRepository;
            _importAuditInsertRepository = importAuditInsertRepository;
        }

        public async Task ReloadStandardsCache()
        {
            var timeStarted = DateTime.UtcNow;
            var (success, standardList) = await _courseManagementOuterApiClient.Get<StandardList>("lookup/standards");
            if (!success || !standardList.Standards.Any())
            {
                _logger.LogError($"ReloadStandardsCacheFunction function failed to get active standards");
                throw new InvalidOperationException("No standards were retrieved from courses api");
            }

            var standardsToReload = standardList.Standards.Select(standard => (Domain.Entities.Standard)standard).ToList();

            await _reloadStandardsRepository.ReloadStandards(standardsToReload);

            _logger.LogInformation("Standards reload complete");
            await _importAuditInsertRepository.Insert(new ImportAudit(timeStarted, standardsToReload.Count, ImportType.Standards));
            _logger.LogInformation("Standards reload audit record written");

        }
    }
}
