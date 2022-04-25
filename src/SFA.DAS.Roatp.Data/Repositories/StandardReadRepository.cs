using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class StandardReadRepository : IStandardReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public StandardReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        public async Task<List<Standard>> GetAllStandards()
        {
            return await _roatpDataContext
                .Standards
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Standard> GetStandard(int larsCode)
        {
            return await _roatpDataContext
                .Standards
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.LarsCode == larsCode);
        }
    }
}
