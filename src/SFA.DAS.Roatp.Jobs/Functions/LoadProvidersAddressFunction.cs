using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class LoadProvidersAddressFunction
{
    private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;
    private readonly ILogger<LoadProvidersAddressFunction> _logger;

    public LoadProvidersAddressFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService, ILogger<LoadProvidersAddressFunction> logger)
    {
        _loadUkrlpAddressesService = loadUkrlpAddressesService;
        _logger = logger;
    }

    [Function(nameof(LoadProvidersAddressFunction))]
    public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%", RunOnStartup = true)] TimerInfo myTimer)
    {
        var result = await _loadUkrlpAddressesService.LoadProvidersAddresses();

        if (result)
            _logger.LogInformation("Ukrlp Addresses updated from last update date");
        else
            _logger.LogWarning("Ukrlp addresses not updated from last update date");
    }
}
