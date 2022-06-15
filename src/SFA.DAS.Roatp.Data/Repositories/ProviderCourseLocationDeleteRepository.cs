using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseLocationDeleteRepository : IProviderCourseLocationDeleteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationDeleteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<int> BulkDelete(int ukprn, int larsCode, bool deleteProviderLocation)
        {
            var query = _roatpDataContext.ProviderCoursesLocations
                .Where(l => l.Course.LarsCode == larsCode && l.Course.Provider.Ukprn == ukprn);

            if (deleteProviderLocation)
                query = query.Where(l => l.Location.LocationType == LocationType.Provider);
            else
                query = query.Where(l => l.Location.LocationType != LocationType.Provider);

            var locations = await query.ToListAsync();

            _roatpDataContext.RemoveRange(locations);
            await _roatpDataContext.SaveChangesAsync();

            return locations.Count;
        }
    }
}
