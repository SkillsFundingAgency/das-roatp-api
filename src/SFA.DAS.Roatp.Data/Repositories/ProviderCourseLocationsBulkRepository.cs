using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseLocationsBulkRepository : IProviderCourseLocationsBulkRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationsBulkRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task BulkInsert(IEnumerable<ProviderCourseLocation> providerCourseLocations)
        {
            await _roatpDataContext.ProviderCoursesLocations.AddRangeAsync(providerCourseLocations);

            await _roatpDataContext.SaveChangesAsync();
        }
        public async Task BulkDelete(IEnumerable<int> providerCourseLocationIds)
        {
            var providerCourseLocations = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => providerCourseLocationIds.Contains(l.Id))
                .ToListAsync();

            _roatpDataContext.ProviderCoursesLocations.RemoveRange(providerCourseLocations);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
