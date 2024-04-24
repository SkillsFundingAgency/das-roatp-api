using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using SFA.DAS.Roatp.Jobs.Services;

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
        public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%")] TimerInfo myTimer)
        {
            await _reloadStandardsCacheService.ReloadStandardsCache();
        }
    }
}
