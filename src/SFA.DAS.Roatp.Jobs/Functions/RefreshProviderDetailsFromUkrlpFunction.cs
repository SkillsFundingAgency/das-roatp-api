using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class RefreshProviderDetailsFromUkrlpFunction(IRefreshProviderDetailsFromUkrlpService _refreshProviderDetailsFromUkrlpService, ILogger<RefreshProviderDetailsFromUkrlpFunction> _logger)
{
    [Function(nameof(RefreshProviderDetailsFromUkrlpFunction))]
    public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%", RunOnStartup = false)] TimerInfo myTimer)
    {
        _logger.LogInformation("LoadProvidersAddressFunction function started");
        await _refreshProviderDetailsFromUkrlpService.RefreshProviderDetailsFromUkrlp(true);
    }
}
