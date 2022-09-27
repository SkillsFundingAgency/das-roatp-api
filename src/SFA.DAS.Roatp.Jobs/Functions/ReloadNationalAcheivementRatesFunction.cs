using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class ReloadNationalAcheivementRatesFunction
    {
        private readonly IReloadNationalAcheivementRatesService _service;

        public ReloadNationalAcheivementRatesFunction(IReloadNationalAcheivementRatesService service)
        {
            _service = service;
        }

        [FunctionName(nameof(ReloadNationalAcheivementRatesFunction))]
        public async Task Run([TimerTrigger("%ReloadNationalAcheivementRatesSchedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("ReloadNationalAcheivementRatesFunction function started");
            await _service.ReloadNationalAcheivementRates();
            log.LogInformation("National Acheivement Rates reload complete");
        }
    }
}
