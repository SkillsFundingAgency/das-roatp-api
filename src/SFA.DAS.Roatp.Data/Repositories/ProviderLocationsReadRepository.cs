using System;
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

        public async Task<ProviderLocation> GetProviderLocation(int ukprn, Guid id)
        {
            return await _roatpDataContext
                .ProviderLocations
                .Include(p => p.Provider)
                .ThenInclude(c => c.Courses)
                .ThenInclude(l => l.Locations)
                .Include(p => p.ProviderCourseLocations)
                .ThenInclude(p => p.ProviderCourse)
                .ThenInclude(s => s.Standard)
                .Where(p => p.Provider.Ukprn == ukprn && p.NavigationId == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<bool> DeletingWillOrphanCourses(int ukprn, Guid id)
        {
            var providerLocation = await _roatpDataContext
                .ProviderLocations
                .Include(p => p.ProviderCourseLocations)
                .Where(p => p.Provider.Ukprn == ukprn && p.NavigationId == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (providerLocation == null)
            {
                return false;
            }

            foreach (var providerCourseId in providerLocation.ProviderCourseLocations.Select(location => location.ProviderCourseId))
            {
                var providerCourseToCheck = await _roatpDataContext.ProviderCourses.
                    Include(p => p.Locations)
                    .ThenInclude(l => l.Location)
                    .Where(x => x.Id == providerCourseId).AsNoTracking().SingleAsync();

                if (providerCourseToCheck.Locations.All(l => l.Location.Id == providerLocation.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
