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
   internal class ProviderCourseLocationsReadRepository : IProviderCourseLocationsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationsReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<ProviderCourseLocation>> GetAllProviderCourseLocations(int ukprn, int larsCode)
        {
            return await _roatpDataContext
                    .ProviderCoursesLocations
                    .Include(l => l.Location)
                    .ThenInclude(r => r.Region)
                    .Where(p => p.ProviderCourse.Provider.Ukprn == ukprn && p.ProviderCourse.LarsCode == larsCode)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<List<ProviderCourseLocation>> GetProviderCourseLocationsByUkprn(int ukprn)
        {
            return await _roatpDataContext
                .ProviderCoursesLocations
                    .Include(l => l.Location)
                    .ThenInclude(r => r.Region)
                    .Where(p => p.ProviderCourse.Provider.Ukprn == ukprn)
                    .AsNoTracking()
                    .ToListAsync();
        }
    }
}
