using System.Linq;
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
        public async Task<Provider> GetProvider(int ukprn)
        {
            return await _roatpDataContext
                .Providers
                .AsNoTracking()
                .Where(c => c.Ukprn == ukprn)
                .SingleOrDefaultAsync();
        }
    }
}
