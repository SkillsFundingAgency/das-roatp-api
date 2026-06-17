using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class RestrictedCourseViewRepository(RoatpDataContext _context) : IRestrictedCourseViewRepository
{
    public async Task<List<RestrictedCourseView>> GetRestrictedCourses(CancellationToken cancellationToken)
    {
        return await _context.RestrictedCourseViews.AsNoTracking().ToListAsync(cancellationToken);
    }
}
