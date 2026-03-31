using System;
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

internal class ProviderCoursesReadRepository : IProviderCoursesReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProviderCoursesReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    [ExcludeFromCodeCoverage]
    public async Task<ProviderCourse> GetProviderCourse(int providerId, string larsCode)
    {
        return await _roatpDataContext
            .ProviderCourses
            .AsNoTracking()
            .Where(c => c.ProviderId == providerId && c.LarsCode == larsCode)
            .SingleOrDefaultAsync();
    }

    [ExcludeFromCodeCoverage]
    public async Task<ProviderCourse> GetProviderCourseByUkprn(int ukprn, string larsCode)
    {
        return await _roatpDataContext
            .ProviderCourses
            .Include(c => c.Standard)
            .Include(c => c.Locations)
            .AsNoTracking()
            .Where(c => c.Provider.Ukprn == ukprn && c.LarsCode == larsCode)
            .SingleOrDefaultAsync();
    }

    [ExcludeFromCodeCoverage]
    public async Task<List<ProviderCourse>> GetAllProviderCourses(int ukprn)
    {
        return await _roatpDataContext
             .ProviderCourses
             .Include(pc => pc.Standard)
             .Include(pc => pc.Locations)
             .Include(pc => pc.Provider)
             .Where(pc => pc.Provider.Ukprn == ukprn && pc.Provider.ProviderCourseTypes.Any(pct => pct.CourseType == pc.Standard.CourseType))
             .AsNoTracking().ToListAsync();
    }

    public async Task<List<ProviderCourse>> GetShortCoursesAddedOnDate(DateTime dateTime, CancellationToken cancellationToken)
    {
        return await _roatpDataContext
            .ProviderCourses
            .Include(pc => pc.Provider)
            .Include(pc => pc.Standard)
            .Where(pc => pc.CreatedDate.Date == dateTime.Date && pc.Standard.CourseType == CourseType.ShortCourse)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UkprnLarsCodeModel>> GetAllShortCourses(CancellationToken cancellationToken)
    {
        return await _roatpDataContext
            .ProviderCourses
            .Include(pc => pc.Provider)
            .Where(pc => pc.Standard.CourseType == CourseType.ShortCourse)
            .AsNoTracking()
            .Select(s => new UkprnLarsCodeModel(s.Provider.Ukprn, s.LarsCode))
            .ToListAsync(cancellationToken);
    }
}
