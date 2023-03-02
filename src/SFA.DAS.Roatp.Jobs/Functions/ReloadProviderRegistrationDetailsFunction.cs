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
        public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadProviderRegistrationDetailsFunction function started");
            await _service.ReloadProviderRegistrationDetails();
            //Below step is dependent on the step above so cannot run it in parallel 
            await _service.ReloadAllAddresses();
            log.LogInformation("Provider registration details reload complete");
        }
    }
}
