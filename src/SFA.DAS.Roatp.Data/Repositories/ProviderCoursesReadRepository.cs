using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Data.Constants;
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

        public async Task<ProviderCourse> GetProviderCourse(int providerId, int larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.ProviderId == providerId && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, int larsCode)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode)
                .SingleOrDefaultAsync();
        }

        public async Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn)
        {
            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.Provider.Ukprn == ukprn)
                .ToListAsync();
        }

        public async Task<int> GetProvidersCount(int larscode)
        {
            var activeProviderRegistrationDetails = await _roatpDataContext
                .ProviderRegistrationDetails
                .AsNoTracking()
                .Where(p=>
                            p.StatusId == OrganisationStatus.Active 
                            || p.StatusId==OrganisationStatus.ActiveNotTakingOnApprentices)
                .ToListAsync();

            var activeProviders = await _roatpDataContext
                .Providers.Where(p=> activeProviderRegistrationDetails
                    .Select(prd => prd.Ukprn).Contains(p.Ukprn))
                    .ToListAsync();

            return await _roatpDataContext
                .ProviderCourses
                .AsNoTracking()
                .Where(c => c.LarsCode == larscode && activeProviders.Select(x=>x.Id).Contains(c.ProviderId))
                .CountAsync();
        }
    }
}
