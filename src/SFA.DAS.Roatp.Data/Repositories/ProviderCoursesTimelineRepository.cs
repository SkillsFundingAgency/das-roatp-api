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
internal class ProviderCoursesTimelineRepository(RoatpDataContext _roatpDataContext) : IProviderCoursesTimelineRepository
{
    public async Task<List<ProviderRegistrationDetail>> GetAllProviderCoursesTimelines(CancellationToken cancellationToken)
    {
        return await _roatpDataContext
            .ProviderRegistrationDetails
            .Include(t => t.Provider)
            .ThenInclude(p => p.ProviderCourseTypes)
            .Include(p => p.Provider)
            .ThenInclude(p => p.ProviderCoursesTimelines)
            .ThenInclude(t => t.Standard)
            .Where(r => r.StatusId == (int)ProviderStatusType.Active || r.StatusId == (int)ProviderStatusType.ActiveNoStarts)
            .ToListAsync(cancellationToken);
    }
}
