using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class ProviderCoursesWriteRepository : IProviderCoursesWriteRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ProviderCoursesWriteRepository> _logger;
    public ProviderCoursesWriteRepository(RoatpDataContext context, ILogger<ProviderCoursesWriteRepository> logger)
    {
        _roatpDataContext = context;
        _logger = logger;
    }

    public async Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity, int ukprn, int larscode, string userId, string userDisplayName, string userAction)
    {
        var providerCourse = await _roatpDataContext.ProviderCourses.FindAsync(patchedProviderCourseEntity.Id);

        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                Audit audit = new(typeof(ProviderCourse).Name, providerCourse.Id.ToString(), userId, userDisplayName, userAction, providerCourse, patchedProviderCourseEntity);

                _roatpDataContext.Audits.Add(audit);

                providerCourse.ContactUsEmail = patchedProviderCourseEntity.ContactUsEmail;
                providerCourse.ContactUsPageUrl = patchedProviderCourseEntity.ContactUsPageUrl;
                providerCourse.ContactUsPhoneNumber = patchedProviderCourseEntity.ContactUsPhoneNumber;
                providerCourse.StandardInfoUrl = patchedProviderCourseEntity.StandardInfoUrl;
                providerCourse.IsApprovedByRegulator = patchedProviderCourseEntity.IsApprovedByRegulator;

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourse update failed for ukprn {Ukprn}, larscode {LarsCode} providerCourseId {ProviderCourseId} by userId {UserId}", ukprn, larscode, patchedProviderCourseEntity.Id, userId);
                throw;
            }
        });

        return providerCourse;
    }

    public async Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse, int ukprn, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                await _roatpDataContext.ProviderCourses.AddAsync(providerCourse);

                Audit audit = new(typeof(ProviderCourse).Name, ukprn.ToString(), userId, userDisplayName, userAction, providerCourse, null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourse Create failed for ukprn {Ukprn}, larscode {LarsCode} by userId {UserId}", ukprn, providerCourse.LarsCode, userId);
                throw;
            }
        });
        return providerCourse;
    }

    public async Task Delete(int ukprn, int larscode, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerCourse = await _roatpDataContext.ProviderCourses
                .Where(l => l.LarsCode == larscode && l.Provider.Ukprn == ukprn)
                .Include(l => l.Locations).Include(l => l.Versions)
                .SingleAsync();

                Audit audit = new(typeof(ProviderCourse).Name, providerCourse.Id.ToString(), userId, userDisplayName, userAction, providerCourse, null);

                _roatpDataContext.Audits.Add(audit);

                _roatpDataContext.Remove(providerCourse);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourse delete failed for ukprn {Ukprn}, larscode {LarsCode} by userId {UserId}", ukprn, larscode, userId);
                throw;
            }
        });
    }
}