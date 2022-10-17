using Microsoft.Azure.WebJobs;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadProvidersUpdatedSinceLastTimeFunction
    {
        private readonly ILoadUkrlpAddressesService _loadUkrlpAddressesService;

        public LoadProvidersUpdatedSinceLastTimeFunction(ILoadUkrlpAddressesService loadUkrlpAddressesService)
        {
            _loadUkrlpAddressesService = loadUkrlpAddressesService;
        }


        [FunctionName(nameof(LoadProvidersUpdatedSinceLastTimeFunction))]
        public async Task Run([TimerTrigger("%UpdateUkrlpDataSchedule%", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("LoadProvidersUpdatedSinceLastTimeFunction function started");
            var result = await _loadUkrlpAddressesService.LoadUkrlpAddressesSinceLastUpdated();

            if (result)
                log.LogInformation("Ukrlp Addresses updated from last update date");
            else
                log.LogWarning("Ukrlp addresses not updated from last update date");

        }
    }
}
