using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.Services
{
    public class GetStandardsCountService : IGetStandardsCountService
    {
        private readonly IGetStandardsCountRepository _getStandardsCountRepository;
        private readonly ILogger<GetStandardsCountService> _logger;
        public GetStandardsCountService(IGetStandardsCountRepository getStandardsCountRepository, ILogger<GetStandardsCountService> logger)
        {
            _getStandardsCountRepository = getStandardsCountRepository;
            _logger = logger;
        }


        public async Task<int> GetStandardsCount()
        {
            var standardsCount = await _getStandardsCountRepository.GetStandardsCount();
            _logger.LogInformation("Gathering standards");
            return standardsCount;
        }
    }
}
