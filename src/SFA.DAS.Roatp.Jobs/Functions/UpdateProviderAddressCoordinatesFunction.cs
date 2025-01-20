using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class UpdateProviderAddressCoordinatesFunction(IUpdateProviderAddressCoordinatesService _updateProviderAddressCoordinatesService, ILogger<UpdateProviderAddressCoordinatesFunction> _logger)
{
    [Function(nameof(UpdateProviderAddressCoordinatesFunction))]
    public async Task Run([TimerTrigger("%UpdateProviderAddressCoordinatesSchedule%", RunOnStartup = false)] TimerInfo myTimer)
    {
        _logger.LogInformation("UpdateProviderAddressCoordinatesFunction function started");
        await _updateProviderAddressCoordinatesService.UpdateProviderAddressCoordinates();
        _logger.LogInformation("UpdateProviderAddressCoordinatesFunction complete");
    }
}
