using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class ReloadStandardsCacheFunction
    {
        private readonly IReloadStandardsCacheService _reloadStandardsCacheService;

        public ReloadStandardsCacheFunction(IReloadStandardsCacheService reloadStandardsCacheService)
        {
            _reloadStandardsCacheService = reloadStandardsCacheService;
        }


        [Function(nameof(ReloadStandardsCacheFunction))]
        public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadStandardsCacheFunction function started");

            await _reloadStandardsCacheService.ReloadStandardsCache();

            log.LogInformation("Standards reload complete");
        }
    }
}
