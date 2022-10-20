using Microsoft.Azure.WebJobs;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class UpdateProviderAddressCoordinatesFunction
    {
        private readonly IReloadStandardsCacheService _reloadStandardsCacheService;

        public UpdateProviderAddressCoordinatesFunction(IReloadStandardsCacheService reloadStandardsCacheService)
        {
            _reloadStandardsCacheService = reloadStandardsCacheService;
        }


        [FunctionName(nameof(UpdateProviderAddressCoordinatesFunction))]
        public async Task Run([TimerTrigger("%UpdateProviderAddressCoordinatesSchedule%",RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("UpdateProviderAddressCoordinatesFunction function started");

            await _reloadStandardsCacheService.ReloadStandardsCache();

            log.LogInformation("UpdateProviderAddressCoordinatesFunction complete");
        }
    }
}
