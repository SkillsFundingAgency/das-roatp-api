using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class ReloadProviderRegistrationDetailsFunction(IReloadProviderRegistrationDetailService _service, ILogger<ReloadProviderRegistrationDetailsFunction> _logger)
{
    [Function(nameof(ReloadProviderRegistrationDetailsFunction))]
    public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%", RunOnStartup = false)] TimerInfo myTimer)
    {
        _logger.LogInformation("ReloadProviderRegistrationDetailsFunction function started");

        await _service.ReloadProviderRegistrationDetails();

        await _service.ReloadAllAddresses();

        await _service.ReloadAllCoordinates();

        await _service.ReloadProviderDetails();

        _logger.LogInformation("ReloadProviderRegistrationDetailsFunction function finished");
    }
}
