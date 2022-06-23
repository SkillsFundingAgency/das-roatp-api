using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderLocationsDeleteRepository : IProviderLocationsDeleteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderLocationsDeleteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task BulkDelete(IEnumerable<int> providerLocationIds)
        {
            try
            {
                var providerLocations = await _roatpDataContext.ProviderLocations
                    .Where(l => providerLocationIds.Contains(l.Id))
                    .ToListAsync();

                _roatpDataContext.ProviderLocations.RemoveRange(providerLocations);

                await _roatpDataContext.SaveChangesAsync();
            }
            catch(Exception e)
            {

            }
        }
    }
}
