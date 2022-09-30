using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;

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
        public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%", RunOnStartup =true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadProviderRegistrationDetailsFunction function started");
            await _service.ReloadProviderRegistrationDetails();
            log.LogInformation("Provider registration details reload complete");
        }
    }
}
