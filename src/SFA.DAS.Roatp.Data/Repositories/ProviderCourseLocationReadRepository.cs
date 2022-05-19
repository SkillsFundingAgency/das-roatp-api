using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
   internal class ProviderCourseLocationReadRepository : IProviderCourseLocationReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<ProviderCourseLocation>> GetAllProviderCourseLocations(int providerCourseId)
        {
            return await _roatpDataContext
                .ProviderCoursesLocations
                .Where(p => p.ProviderCourseId == providerCourseId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
