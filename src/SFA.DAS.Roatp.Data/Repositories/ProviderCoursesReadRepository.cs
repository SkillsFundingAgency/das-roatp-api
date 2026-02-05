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
    internal class ProviderCoursesReadRepository : IProviderCoursesReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCoursesReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourse> GetProviderCourse(int providerId, string larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.ProviderId == providerId && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, string larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .Include(c => c.Standard)
                .Include(c => c.Locations)
                .AsNoTracking()
                .Where(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn)
        {
            return await _roatpDataContext
                 .ProviderCourses
                 .Include(pc => pc.Standard)
                 .Include(pc => pc.Locations)
                 .Include(pc => pc.Provider)
                 .Where(pc => pc.Provider.Ukprn == ukprn && pc.Provider.ProviderCourseTypes.Any(pct => pct.CourseType == pc.Standard.CourseType))
                 .AsNoTracking().ToListAsync();
        }
    }
}
