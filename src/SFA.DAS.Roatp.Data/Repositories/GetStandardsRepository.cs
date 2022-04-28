using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class GetStandardsRepository : IGetStandardsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<GetStandardsRepository> _logger;

        public GetStandardsRepository(RoatpDataContext roatpDataContext, ILogger<GetStandardsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }


        public async Task<List<Standard>> GetStandards()
        {
            _logger.LogInformation("GetStandards invoked");
            return await _roatpDataContext.Standards.ToListAsync();
        }
    }
}