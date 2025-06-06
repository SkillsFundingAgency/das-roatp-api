using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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
                .Where(p => p.Provider.Ukprn == ukprn && p.NavigationId == id)
                    .AsNoTracking()
                .SingleOrDefaultAsync();
        }
    }
}
