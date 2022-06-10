using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class ReloadStandardsCacheFunction
    {
        private readonly IReloadStandardsCacheService _reloadStandardsCacheService;

        public ReloadStandardsCacheFunction(IReloadStandardsCacheService reloadStandardsCacheService)
        {
            _reloadStandardsCacheService = reloadStandardsCacheService;
        }


        [FunctionName(nameof(ReloadStandardsCacheFunction))]
        public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadStandardsCacheFunction function started");

            await _reloadStandardsCacheService.ReloadStandardsCache();

            log.LogInformation("Standards reload complete");
        }
    }
}
