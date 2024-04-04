using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Functions;

public class LoadProvidersAddressFunction
{
    private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;

    public LoadProvidersAddressFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService)
    {
        _loadUkrlpAddressesService = loadUkrlpAddressesService;
    }

    [Function(nameof(LoadProvidersAddressFunction))]
    public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation("LoadProvidersAddressFunction function started");
        var result = await _loadUkrlpAddressesService.LoadProvidersAddresses();

        if (result)
            log.LogInformation("Ukrlp Addresses updated from last update date");
        else
            log.LogWarning("Ukrlp addresses not updated from last update date");
    }
}
