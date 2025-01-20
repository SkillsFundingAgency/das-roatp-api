using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Roatp.Application.Mediatr.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<RequestPerformanceBehaviour<TRequest, TResponse>> _logger;

        public RequestPerformanceBehaviour(ILogger<RequestPerformanceBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var timer = Stopwatch.StartNew();

            var response = await next();

            timer.Stop();

            var elapsedTime = timer.ElapsedMilliseconds;
            var name = typeof(TRequest).Name;

            _logger.LogInformation("Command: {Name}, ElapsedTime: {ElapsedTime} milliseconds", name, elapsedTime);

            return response;
        }
    }
}
