using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class UpdateProviderAddressCoordinatesFunction
    {
        private readonly IUpdateProviderAddressCoordinatesService _updateProviderAddressCoordinatesService;

        public UpdateProviderAddressCoordinatesFunction(IUpdateProviderAddressCoordinatesService updateProviderAddressCoordinatesService)
        {
            _updateProviderAddressCoordinatesService = updateProviderAddressCoordinatesService;
        }

        [Function(nameof(UpdateProviderAddressCoordinatesFunction))]
        public async Task Run([TimerTrigger("%UpdateProviderAddressCoordinatesSchedule%", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("UpdateProviderAddressCoordinatesFunction function started");
            await _updateProviderAddressCoordinatesService.UpdateProviderAddressCoordinates();
            log.LogInformation("UpdateProviderAddressCoordinatesFunction complete");
        }
    }
}
