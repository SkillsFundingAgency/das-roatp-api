using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderCourseLocationsInsertRepository : IProviderCourseLocationsInsertRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationsInsertRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task BulkInsert(IEnumerable<ProviderCourseLocation> providerCourseLocations)
        {
            await _roatpDataContext.ProviderCoursesLocations.AddRangeAsync(providerCourseLocations);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
