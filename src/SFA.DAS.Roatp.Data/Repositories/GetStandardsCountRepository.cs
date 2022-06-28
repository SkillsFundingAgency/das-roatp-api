using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class GetStandardsCountRepository : IGetStandardsCountRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<GetStandardsCountRepository> _logger;

        public GetStandardsCountRepository(RoatpDataContext roatpDataContext, ILogger<GetStandardsCountRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }


        public async Task<int> GetStandardsCount()
        {
            _logger.LogInformation("GetStandardsCount invoked");
            return await _roatpDataContext.Standards.CountAsync();
        }
    }
}