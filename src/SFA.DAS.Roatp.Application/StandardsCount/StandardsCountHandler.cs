using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.ReloadStandards;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.StandardsCount
{
    public class StandardsCountHandler : IRequestHandler<StandardsCountRequest, int>
    {
        private readonly IGetStandardsCountRepository _getStandardsCountRepository;
        private readonly ILogger<StandardsCountHandler> _logger;
        public StandardsCountHandler(IGetStandardsCountRepository getStandardsCountRepository, ILogger<StandardsCountHandler> logger)
        {
            _getStandardsCountRepository = getStandardsCountRepository;
            _logger = logger;
        }

        public async Task<int> Handle(StandardsCountRequest request, CancellationToken cancellationToken)
        {
            var standardsCount = await _getStandardsCountRepository.GetStandardsCount();
            _logger.LogInformation("Gathering standards count:{standardsCount}", standardsCount);
            return standardsCount;
        }
    }
}
