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
internal class ProviderCourseTypesRepository : IProviderCourseTypesRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public ProviderCourseTypesRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<ProviderCourseType>> GetProviderCourseTypesByUkprn(int ukprn, CancellationToken cancellationToken = default)
    {
        return await _roatpDataContext
            .ProviderCoursesTypes
            .Where(p => p.Ukprn == ukprn)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<int>> GetAllProvidersWithShortCourses(CancellationToken cancellationToken = default)
    {
        return await _roatpDataContext
            .ProviderCoursesTypes
            .Where(p => p.CourseType == Domain.Models.CourseType.ShortCourse)
            .Select(p => p.Ukprn)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProviderCourseType>> GetAllProvidersByCourseType(CourseType courseType, CancellationToken cancellationToken = default)
    {
        return await _roatpDataContext
            .ProviderCoursesTypes
            .Include(x => x.Provider)
            .Where(p => p.CourseType == courseType)
            .ToListAsync(cancellationToken);
    }

    public async Task RestrictProvider(int ukprn, CourseType courseType, List<ProviderAllowedCourse> coursesToAdd, List<ProviderAllowedCourse> coursesToRemove, string userId, string userDisplayName, CancellationToken cancellationToken = default)
    {
        ProviderCourseType providerCourseType = await _roatpDataContext.ProviderCoursesTypes
            .FirstOrDefaultAsync(p => p.Ukprn == ukprn && p.CourseType == courseType, cancellationToken);

        bool initialRestrictedState = providerCourseType.IsRestrictedProvider;

        providerCourseType.IsRestrictedProvider = true;

        _roatpDataContext.Audits.Add(new Audit(
            nameof(ProviderCourseType),
            ukprn.ToString(),
            userId,
            userDisplayName,
            "UpdateProviderCourseType",
            new ProviderCourseType
            {
                Ukprn = providerCourseType.Ukprn,
                CourseType = providerCourseType.CourseType,
                IsRestrictedProvider = initialRestrictedState
            },
            new ProviderCourseType
            {
                Ukprn = providerCourseType.Ukprn,
                CourseType = providerCourseType.CourseType,
                IsRestrictedProvider = providerCourseType.IsRestrictedProvider
            }));

        List<ProviderAllowedCourse> initialState = await _roatpDataContext.ProviderAllowedCourses
            .Where(pac => pac.Ukprn == ukprn)
            .ToListAsync(cancellationToken);

        if (coursesToAdd.Count > 0)
        {
            await _roatpDataContext.ProviderAllowedCourses
                .AddRangeAsync(coursesToAdd, cancellationToken);
        }

        if (coursesToRemove.Count > 0)
        {
            _roatpDataContext.ProviderAllowedCourses
                .RemoveRange(coursesToRemove);
        }

        if (coursesToAdd.Count > 0 || coursesToRemove.Count > 0)
        {
            var updatedState = initialState
                .Where(pac => !coursesToRemove.Any(x =>
                    x.Ukprn == pac.Ukprn &&
                    x.LarsCode == pac.LarsCode))
                .Concat(coursesToAdd)
                .ToList();

            _roatpDataContext.Audits.Add(new Audit(
            nameof(ProviderAllowedCourse),
            ukprn.ToString(),
            userId,
            userDisplayName,
            "UpdateProviderAllowedCourse",
            initialState,
            updatedState));
        }

        await _roatpDataContext.SaveChangesAsync(cancellationToken);
    }
}
