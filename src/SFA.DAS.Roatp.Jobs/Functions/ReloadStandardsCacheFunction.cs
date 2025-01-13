using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class ReloadStandardsCacheFunction(IReloadStandardsCacheService _reloadStandardsCacheService, ILogger<ReloadStandardsCacheFunction> _logger)
{
    [Function(nameof(ReloadStandardsCacheFunction))]
    public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%", RunOnStartup = true)] TimerInfo myTimer)
    {
        _logger.LogInformation("ReloadStandardsCacheFunction function started");

        await _reloadStandardsCacheService.ReloadStandardsCache();

        _logger.LogInformation("Standards reload complete");
    }
}
