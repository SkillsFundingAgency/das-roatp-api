using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderCourseTypesReadRepository : IProviderCourseTypesReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProviderCourseTypesReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<ProviderCourseType>> GetProviderCourseTypesByUkprn(int ukprn)
    {
        return await _roatpDataContext
            .ProviderCoursesTypes
            .Where(p => p.Ukprn == ukprn)
            .AsNoTracking()
            .ToListAsync();
    }
}