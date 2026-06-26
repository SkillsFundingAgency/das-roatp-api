using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderAllowedCoursesRepository(RoatpDataContext _roatpDataContext) : IProviderAllowedCoursesRepository
{
    public async Task<List<ProviderAllowedCourse>> GetProviderAllowedCourses(int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _roatpDataContext.ProviderAllowedCourses
            .Include(p => p.Standard)
            .AsNoTracking()
            .Where(p => p.Ukprn == ukprn && p.Standard.CourseType == courseType)
            .ToListAsync(cancellationToken);
        return providerAllowedCourses;
    }
}
