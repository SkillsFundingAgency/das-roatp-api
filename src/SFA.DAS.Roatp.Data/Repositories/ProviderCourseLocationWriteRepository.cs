using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseLocationWriteRepository : IProviderCourseLocationWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation)
        {
            _roatpDataContext.ProviderCoursesLocations.Add(providerCourseLocation);
            await _roatpDataContext.SaveChangesAsync();
            return providerCourseLocation;
        }
    }
}
