using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class LoadProvidersAddressFunction(ILoadUkrlpAddressesService _loadUkrlpAddressesService, ILogger<LoadProvidersAddressFunction> _logger)
{
    [Function(nameof(LoadProvidersAddressFunction))]
    public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%", RunOnStartup = false)] TimerInfo myTimer)
    {
        _logger.LogInformation("LoadProvidersAddressFunction function started");
        var result = await _loadUkrlpAddressesService.LoadProvidersAddresses();

        if (result)
            _logger.LogInformation("Ukrlp Addresses updated from last update date");
        else
            _logger.LogWarning("Ukrlp addresses not updated from last update date");
    }
}
