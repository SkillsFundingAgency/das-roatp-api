using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCourseDeleteRepository : IProviderCourseDeleteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseDeleteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }
       
        public async Task Delete(int ukprn, int larscode)
        {
            var providerCourse = await _roatpDataContext.ProviderCourses
                .Where(l => l.LarsCode == larscode && l.Provider.Ukprn == ukprn)
                .Include(l=>l.Locations).Include(l=>l.Versions)
                .SingleAsync();

            _roatpDataContext.Remove(providerCourse);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
