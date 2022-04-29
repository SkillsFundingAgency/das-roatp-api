using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Roatp.Api.Services;

namespace SFA.DAS.Roatp.Api.HealthCheck
{
    public class StandardsHealthCheck:IHealthCheck
    {
        public const string HealthCheckResultDescription = "Standards Health Check";
        private readonly IGetStandardsCountService _getStandardsCountService;

        public StandardsHealthCheck(IGetStandardsCountService getStandardsCountService)
        {
            _getStandardsCountService = getStandardsCountService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var standardsCount = await _getStandardsCountService.GetStandardsCount();
            return standardsCount==0 
                ? HealthCheckResult.Unhealthy(HealthCheckResultDescription) 
                : HealthCheckResult.Healthy(HealthCheckResultDescription);
        }
    }
}
