using System;
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

        public Task<ProviderCourse> GetProviderCourseDeliveryModels(int ukprn, int larsCode)
        {
            return _roatpDataContext
                .ProviderCourses
                .Include(c => c.Provider)
                .Include(c => c.Locations)
                .Include(c => c.Versions)
                .SingleOrDefaultAsync(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode);
            throw new NotImplementedException();
        }
    }
}
