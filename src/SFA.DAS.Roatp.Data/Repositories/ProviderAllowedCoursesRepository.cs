using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories;

internal class ProviderAllowedCoursesRepository(RoatpDataContext _roatpDataContext) : IProviderAllowedCoursesRepository
{
    private const string CreateProviderCourseType = "CreateProviderCourseType";
    private const string UpdateProviderAllowedCourse = "UpdateProviderAllowedCourse";
    private const string CreateProviderAllowedCourse = "CreateProviderAllowedCourse";
    public async Task<List<ProviderAllowedCourse>> GetProviderAllowedCourses(int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _roatpDataContext.ProviderAllowedCourses
            .Include(p => p.Standard)
            .Include(p => p.ProviderCourse)
            .AsNoTracking()
            .Where(p => p.Ukprn == ukprn && p.Standard.CourseType == courseType)
            .ToListAsync(cancellationToken);
        return providerAllowedCourses;
    }

    public async Task<List<ProviderAllowedCourse>> GetProviderAllowedCoursesByLarsCode(string larsCode, CancellationToken cancellationToken)
    {
        var providerAllowedCourses = await _roatpDataContext.ProviderAllowedCourses
            .Include(x => x.Provider)
            .Where(p => p.LarsCode == larsCode)
            .AsNoTracking().ToListAsync(cancellationToken);
        return providerAllowedCourses;
    }

    public async Task UpsertProviderAllowedCourse(int ukprn, string larsCode, CourseType courseType, DateTime? lastDateStarts, string userId, string userDisplayName)
    {
        Provider provider = await _roatpDataContext.Providers
            .Include(p => p.ProviderCourseTypes)
            .Include(c => c.ProviderAllowedCourses)
            .FirstOrDefaultAsync(p => p.Ukprn == ukprn);

        bool providerAllowedCourseUpdated = false;

        DateTime? existingLastDateStarts = null;

        if (!provider.ProviderCourseTypes.Any(ct => ct.CourseType == courseType))
        {
            provider.ProviderCourseTypes.Add(new ProviderCourseType
            {
                Ukprn = ukprn,
                CourseType = courseType
            });

            _roatpDataContext.Audits.Add(new Audit(
                nameof(ProviderCourseType),
                ukprn.ToString(),
                userId,
                userDisplayName,
                CreateProviderCourseType,
                new ProviderCourseType
                {
                    Ukprn = ukprn,
                    CourseType = courseType
                },
                null));
        }

        var existingAllowedCourse = provider.ProviderAllowedCourses.FirstOrDefault(p => p.LarsCode == larsCode);

        if (existingAllowedCourse != null)
        {
            existingLastDateStarts = existingAllowedCourse.LastDateStarts;

            existingAllowedCourse.LastDateStarts = lastDateStarts;

            providerAllowedCourseUpdated = true;
        }
        else
        {
            provider.ProviderAllowedCourses.Add(new ProviderAllowedCourse
            {
                LarsCode = larsCode,
                Ukprn = ukprn,
                LastDateStarts = lastDateStarts
            });
        }

        _roatpDataContext.Audits.Add(new Audit(
            nameof(ProviderAllowedCourse),
            ukprn.ToString(),
            userId,
            userDisplayName,
            providerAllowedCourseUpdated ? UpdateProviderAllowedCourse : CreateProviderAllowedCourse,
            new ProviderAllowedCourse
            {
                LarsCode = larsCode,
                Ukprn = ukprn,
                LastDateStarts = providerAllowedCourseUpdated ? existingLastDateStarts : lastDateStarts
            },
            providerAllowedCourseUpdated ? new ProviderAllowedCourse
            {
                LarsCode = larsCode,
                Ukprn = ukprn,
                LastDateStarts = lastDateStarts
            } : null));

        await _roatpDataContext.SaveChangesAsync();
    }

    public async Task PatchProviderAllowedCourse(int ukprn, string larsCode, DateTime? lastDateStarts, string userId, string userDisplayName)
    {
        ProviderAllowedCourse providerAllowedCourse = await _roatpDataContext.ProviderAllowedCourses
            .FirstOrDefaultAsync(pac => pac.Ukprn == ukprn && pac.LarsCode == larsCode);

        DateTime? existingLastDateStarts = providerAllowedCourse.LastDateStarts;

        providerAllowedCourse.LastDateStarts = lastDateStarts;

        _roatpDataContext.Audits.Add(new Audit(
            nameof(ProviderAllowedCourse),
            ukprn.ToString(),
            userId,
            userDisplayName,
            UpdateProviderAllowedCourse,
            new ProviderAllowedCourse
            {
                LarsCode = larsCode,
                Ukprn = ukprn,
                LastDateStarts = existingLastDateStarts
            },
            new ProviderAllowedCourse
            {
                LarsCode = larsCode,
                Ukprn = ukprn,
                LastDateStarts = lastDateStarts
            }));

        await _roatpDataContext.SaveChangesAsync();
    }
}
