using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProvidersReadRepository : IProvidersReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProvidersReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
        public async Task<Provider> GetByUkprn(int ukprn)
        {
            return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .AsNoTracking().SingleOrDefaultAsync(p => p.Ukprn == ukprn);
        }

        public async Task<List<Provider>> GetAllProviders()
        {
            return await _roatpDataContext.Providers
                        .Include(p => p.ProviderAddress)
                        .AsNoTracking().ToListAsync();
        }
    }
}
