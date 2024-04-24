using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
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

        [Function(nameof(UpdateProviderAddressCoordinatesFunction))]
        public async Task Run([TimerTrigger("%UpdateProviderAddressCoordinatesSchedule%", RunOnStartup = false)] TimerInfo myTimer)
        {
            await _updateProviderAddressCoordinatesService.UpdateProviderAddressCoordinates();
        }
    }
}
