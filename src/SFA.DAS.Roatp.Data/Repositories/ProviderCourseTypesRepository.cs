using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderCourseTypesRepository : IProviderCourseTypesRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ProviderCourseTypesRepository> _logger;

    public ProviderCourseTypesRepository(RoatpDataContext roatpDataContext, ILogger<ProviderCourseTypesRepository> logger)
    {
        _roatpDataContext = roatpDataContext;
        _logger = logger;
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

        List<ProviderAllowedCourse> providerAllowedCourse = await _roatpDataContext.ProviderAllowedCourses
            .Where(pac => pac.Ukprn == ukprn)
            .ToListAsync(cancellationToken);

        if (coursesToAdd.Count > 0)
        {
            await _roatpDataContext.ProviderAllowedCourses
                .AddRangeAsync(coursesToAdd, cancellationToken);
        }

        if (coursesToRemove.Count > 0)
        {
            var providerCoursesToRemove = providerAllowedCourse
                .Where(x => coursesToRemove.Any(y => y.Id == x.Id))
                .ToList();

            _roatpDataContext.ProviderAllowedCourses
                .RemoveRange(providerCoursesToRemove);
        }

        if (coursesToAdd.Count > 0 || coursesToRemove.Count > 0)
        {
            var updatedState = providerAllowedCourse
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
            providerAllowedCourse,
            updatedState));
        }

        await _roatpDataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateProviderCourseType(IEnumerable<ProviderCourseType> providerCourseType, string userId, string userDisplayName, int ukprn, string userAction, CancellationToken cancellationToken)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _roatpDataContext.ProviderCoursesTypes.AddRangeAsync(providerCourseType, cancellationToken);

                Audit audit = new(nameof(ProviderCourseType), ukprn.ToString(), userId, userDisplayName, userAction, providerCourseType, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Failed to create provider course types for ukprn {Ukprn}", ukprn);
                throw new InvalidOperationException();
            }
        });
    }
}
