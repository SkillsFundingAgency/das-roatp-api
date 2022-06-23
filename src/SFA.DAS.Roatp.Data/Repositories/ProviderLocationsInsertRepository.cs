using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderLocationsInsertRepository : IProviderLocationsInsertRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderLocationsInsertRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task BulkInsert(IEnumerable<ProviderLocation> providerLocations)
        {
            await _roatpDataContext.ProviderLocations.AddRangeAsync(providerLocations);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
