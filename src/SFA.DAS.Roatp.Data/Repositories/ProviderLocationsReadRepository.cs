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

        public async Task<List<ProviderCourse>> GetProviderCoursesByLocation(int ukprn, Guid id)
        {
            var providerLocation = await _roatpDataContext
                    .ProviderLocations
                    .Include(p => p.ProviderCourseLocations)
                    .ThenInclude(p => p.ProviderCourse)
                    .ThenInclude(p => p.Locations)
                    .ThenInclude(l => l.Location)
                    .Where(p => p.Provider.Ukprn == ukprn && p.NavigationId == id)
                    .SingleOrDefaultAsync();


            if (providerLocation == null)
            {
                return new List<ProviderCourse>();
            }

            return providerLocation.ProviderCourseLocations.Select(providerCourseLocation => providerCourseLocation.ProviderCourse).ToList();
        }
    }
}
