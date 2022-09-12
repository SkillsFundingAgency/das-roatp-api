using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class StandardsReadRepository : IStandardsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public StandardsReadRepository(RoatpDataContext roatpDataContext)
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
