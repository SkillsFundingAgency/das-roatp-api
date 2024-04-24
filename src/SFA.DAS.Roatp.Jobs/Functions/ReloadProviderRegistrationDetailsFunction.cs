using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
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

        [Function(nameof(ReloadProviderRegistrationDetailsFunction))]
        public async Task Run([TimerTrigger("%ReloadProviderRegistrationDetailsSchedule%", RunOnStartup = false)] TimerInfo myTimer)
        {
            await _service.ReloadProviderRegistrationDetails();
            await _service.ReloadAllAddresses();
            await _service.ReloadAllCoordinates();
        }
    }
}
