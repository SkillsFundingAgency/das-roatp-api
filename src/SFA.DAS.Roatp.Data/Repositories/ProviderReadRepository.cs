using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class ProviderReadRepository : IProviderReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        public async Task<Provider> GetByUkprn(int ukprn)
        {
            return await _roatpDataContext.Providers.AsNoTracking().SingleOrDefaultAsync(p => p.Ukprn == ukprn);
        }

        public async Task<List<Provider>> GetAll()
        {
            return await _roatpDataContext.Providers.AsNoTracking().ToListAsync();
        }
    }
}
