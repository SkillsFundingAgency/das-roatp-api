using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseLocationsWriteRepository : IProviderCourseLocationsWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public ProviderCourseLocationsWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation)
        {
            _roatpDataContext.ProviderCoursesLocations.Add(providerCourseLocation);
            await _roatpDataContext.SaveChangesAsync();
            return providerCourseLocation;
        }

        public async Task Delete(Guid navigationId)
        {
            var location = await _roatpDataContext.ProviderCoursesLocations
                .Where(l => l.NavigationId == navigationId)
                .SingleAsync();

            _roatpDataContext.Remove(location);

            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
