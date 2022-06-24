using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderLocationsReadRepository : IProviderLocationsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderLocationsReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<ProviderLocation>> GetAllProviderLocations(int ukprn)
        {
            return await _roatpDataContext
                .ProviderLocations
                .Where(p => p.Provider.Ukprn == ukprn)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ProviderLocation>> GetProviderLocationsById(int providerLocationId)
        {
            return await _roatpDataContext
                .ProviderLocations
                .Where(p => p.Id == providerLocationId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
