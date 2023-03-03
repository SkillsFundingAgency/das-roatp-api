using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class UpdateProviderAddressCoordinatesFunction
    {
        private readonly IUpdateProviderAddressCoordinatesService _updateProviderAddressCoordinatesService;

        public UpdateProviderAddressCoordinatesFunction(IUpdateProviderAddressCoordinatesService updateProviderAddressCoordinatesService)
        {
            _updateProviderAddressCoordinatesService = updateProviderAddressCoordinatesService;
        }

        [FunctionName(nameof(UpdateProviderAddressCoordinatesFunction))]
        public async Task Run([TimerTrigger("%UpdateProviderAddressCoordinatesSchedule%", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("UpdateProviderAddressCoordinatesFunction function started");
            await _updateProviderAddressCoordinatesService.UpdateProviderAddressCoordinates();
            log.LogInformation("UpdateProviderAddressCoordinatesFunction complete");
        }
    }
}
