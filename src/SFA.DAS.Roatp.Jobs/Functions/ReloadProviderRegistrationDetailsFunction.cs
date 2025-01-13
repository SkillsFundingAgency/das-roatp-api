using System.Diagnostics;
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
        Stopwatch stopwatch = Stopwatch.StartNew();
        await _service.ReloadProviderRegistrationDetails();
        _logger.LogInformation("Reload register completed in {TotalMilliseconds} ms", stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        await _service.ReloadAllAddresses();
        _logger.LogInformation("Reload registered provider address completed in {TotalMilliseconds} ms", stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        await _service.ReloadAllCoordinates();
        _logger.LogInformation("Reload register provider address coordinates completed in {TotalMilliseconds} ms", stopwatch.ElapsedMilliseconds);

        _logger.LogInformation("ReloadProviderRegistrationDetailsFunction function finished");
    }
}
