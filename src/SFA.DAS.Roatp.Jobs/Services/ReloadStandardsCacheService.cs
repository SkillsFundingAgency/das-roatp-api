using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ReloadStandardsCacheService : IReloadStandardsCacheService
{
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly IReloadStandardsRepository _reloadStandardsRepository;
    private readonly IImportAuditWriteRepository _importAuditWriteRepository;
    private readonly ILogger<ReloadStandardsCacheService> _logger;

    public ReloadStandardsCacheService(ILogger<ReloadStandardsCacheService> logger, ICourseManagementOuterApiClient courseManagementOuterApiClient, IReloadStandardsRepository reloadStandardsRepository, IImportAuditWriteRepository importAuditWriteRepository)
    {
        _logger = logger;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _reloadStandardsRepository = reloadStandardsRepository;
        _importAuditWriteRepository = importAuditWriteRepository;
    }

    public async Task ReloadStandardsCache()
    {
        var timeStarted = DateTime.UtcNow;
        var (success, standardList) = await _courseManagementOuterApiClient.Get<StandardList>("lookup/standards");
        if (!success || !standardList.Standards.Any())
        {
            _logger.LogError("ReloadStandardsCacheFunction function failed to get active standards");
            throw new InvalidOperationException("No standards were retrieved from courses api");
        }

        var standardsToReload = standardList.Standards.Select(standardModel => (Standard)standardModel).ToList();

        await _reloadStandardsRepository.ReloadStandards(standardsToReload);

        _logger.LogInformation("Standards reload complete");
        await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, standardsToReload.Count, ImportType.Standards));
    }
}
