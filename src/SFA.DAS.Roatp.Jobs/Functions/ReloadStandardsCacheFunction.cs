using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class ReloadStandardsCacheFunction
    {
        private readonly IGetActiveStandardsApiClient _getActiveStandardsApiClient;
        private readonly IReloadStandardsApiClient _reloadStandardsApiClient;


        public ReloadStandardsCacheFunction(IGetActiveStandardsApiClient getActiveStandardsApiClient, IReloadStandardsApiClient reloadStandardsApiClient)
        {
            _getActiveStandardsApiClient = getActiveStandardsApiClient;
            _reloadStandardsApiClient = reloadStandardsApiClient;
        }


        [FunctionName(nameof(ReloadStandardsCacheFunction))]
        public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"ReloadStandardsCacheFunction function started");

            var standardList = await _getActiveStandardsApiClient.GetActiveStandards();
            if (standardList == null)
            {
                log.LogError($"ReloadStandardsCacheFunction function failed to get active standards");
                return;
            }

            var standardsRequest = new StandardsRequest { Standards = standardList.Standards };
            var result = await _reloadStandardsApiClient.ReloadStandardsDetails(standardsRequest);
            if (result == HttpStatusCode.OK)
                log.LogInformation($"ReloadStandardsCacheFunction function completed");
            else
            {
                log.LogError($"ReloadStandardsCacheFunction function failed", result);
            }

        }
    }
}
