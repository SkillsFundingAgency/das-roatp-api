using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseLocationsDeleteRepository : IProviderCourseLocationsDeleteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationsDeleteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task BulkDelete(IEnumerable<int> providerCourseLocationIds)
        {
            var providerCourseLocations = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => providerCourseLocationIds.Contains(l.Id))
                .ToListAsync();

            _roatpDataContext.ProviderCoursesLocations.RemoveRange(providerCourseLocations);

            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task Delete(Guid navigationId)
        {
            var location = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => l.NavigationId == navigationId)
                .SingleAsync();

            _roatpDataContext.Remove(location);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
