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
        public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadProviderRegistrationDetailsFunction function started");

            // fullImport clear down 
            // partialImport clear down - clear down everything that's not pilot
            await _service.ReloadProviderRegistrationDetails();

            log.LogInformation("Provider registration details reload complete");
        }
    }
}
