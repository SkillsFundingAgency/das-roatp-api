using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class ProviderRepository : IProviderRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderRepository(RoatpDataContext roatpDataContext)
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

        public async Task<Provider> UpdateProvider(int ukprn, bool hasConfirmedDetails)
        {
            var providertoUpdate = _roatpDataContext
                .Providers
                .AsNoTracking()
                .Where(c => c.Ukprn == ukprn)
                .SingleOrDefaultAsync().Result;

            if (providertoUpdate != null)
            {
                providertoUpdate.HasConfirmedDetails = hasConfirmedDetails;
                await _roatpDataContext.SaveChangesAsync();
            }

            return providertoUpdate;
        }
    }
}
