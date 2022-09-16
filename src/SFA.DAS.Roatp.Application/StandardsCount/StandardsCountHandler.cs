using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.StandardsCount
{
    public class StandardsCountHandler : IRequestHandler<StandardsCountRequest, int>
    {
        private readonly IStandardsReadRepository _standardsReadRepository;
        private readonly ILogger<StandardsCountHandler> _logger;
        public StandardsCountHandler(IStandardsReadRepository standardsReadRepository, ILogger<StandardsCountHandler> logger)
        {
            _standardsReadRepository = standardsReadRepository;
            _logger = logger;
        }

        public async Task<int> Handle(StandardsCountRequest request, CancellationToken cancellationToken)
        {
            var standardsCount = await _standardsReadRepository.GetStandardsCount();
            _logger.LogInformation("Gathering standards count:{standardsCount}", standardsCount);
            return standardsCount;
        }
    }
}
