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
        private readonly IGetStandardsService _getStandardsService;

        public StandardsHealthCheck(IGetStandardsService getStandardsService)
        {
            _getStandardsService = getStandardsService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var standards = await _getStandardsService.GetStandards();
            if (standards == null || !standards.Any())
                return HealthCheckResult.Unhealthy(HealthCheckResultDescription);

            return HealthCheckResult.Healthy(HealthCheckResultDescription);
        }
    }
}
