using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class ReloadProviderRegistrationDetailsFunction
    {
        private readonly IReloadProviderRegistrationDetailService _service;

        public ReloadProviderRegistrationDetailsFunction(IReloadProviderRegistrationDetailService service)
        {
            _service = service;
        }

        [FunctionName(nameof(ReloadProviderRegistrationDetailsFunction))]
        public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadProviderRegistrationDetailsFunction function started");

            var startTime = DateTime.Now;
            await _service.ReloadProviderRegistrationDetails();
            log.LogInformation($"Reload register completed in {(DateTime.Now - startTime).TotalMilliseconds} ms");

            startTime = DateTime.Now;
            await _service.ReloadAllAddresses();
            log.LogInformation($"Reload registered provider address completed in {(DateTime.Now - startTime).TotalMilliseconds} ms");

            startTime = DateTime.Now;
            await _service.ReloadAllCoordinates();
            log.LogInformation($"Reload register provider address coordinates completed in {(DateTime.Now - startTime).TotalMilliseconds}ms");

            log.LogInformation($"ReloadProviderRegistrationDetailsFunction function finished");
        }
    }
}
