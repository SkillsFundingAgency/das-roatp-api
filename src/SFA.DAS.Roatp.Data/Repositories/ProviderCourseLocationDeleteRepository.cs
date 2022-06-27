using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
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

        public async Task BulkDelete(IEnumerable<int> providerCourseLocationIds)
        {
            var locations = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => providerCourseLocationIds.Contains(l.Id))
                .ToListAsync();

            _roatpDataContext.RemoveRange(locations);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
