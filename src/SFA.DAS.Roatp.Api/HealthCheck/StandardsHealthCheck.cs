using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Roatp.Application.StandardsCount;

namespace SFA.DAS.Roatp.Api.HealthCheck
{
    public class StandardsHealthCheck : IHealthCheck
    {
        public const string HealthCheckResultDescription = "Standards Health Check";
        private readonly IMediator _mediator;
        public StandardsHealthCheck(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var standardsCount = await _mediator.Send(new StandardsCountRequest(), cancellationToken);
            return standardsCount==0 
                ? HealthCheckResult.Unhealthy(HealthCheckResultDescription) 
                : HealthCheckResult.Healthy(HealthCheckResultDescription);
        }
    }
}
