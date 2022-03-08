using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    internal class GetProviderCourseRepository : IGetProviderCourseRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public GetProviderCourseRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourse> GetProviderCourse(int ukprn, int larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .Include(c => c.Provider)
                .Include(c => c.Locations)
                .Include(c => c.Versions)
                .SingleOrDefaultAsync(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode);
        }

        public async Task<Provider> GetAllProviderCourse(int ukprn)
        {
            return await _roatpDataContext
                .Providers
                .Where(p => p.Ukprn == ukprn)
                .Include(p => p.Courses)
                .ThenInclude(c => c.Locations)
                .SingleOrDefaultAsync();
        }
    }
}
