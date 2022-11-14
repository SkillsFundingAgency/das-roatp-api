using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class ProviderCoursesWriteRepository : AuditWriteRepository<ProviderCourse>, IProviderCoursesWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderCoursesWriteRepository> _logger;
        public ProviderCoursesWriteRepository(RoatpDataContext context, ILogger<ProviderCoursesWriteRepository> logger) : base(context)
        {
            _roatpDataContext = context;
            _logger = logger;
        }

        public async Task<ProviderCourse> PatchProviderCourse(ProviderCourse patchedProviderCourseEntity)
        {
            var providerCourse = await _roatpDataContext
                .ProviderCourses
                .FindAsync(patchedProviderCourseEntity.Id);

            providerCourse.ContactUsEmail = patchedProviderCourseEntity.ContactUsEmail;
            providerCourse.ContactUsPageUrl = patchedProviderCourseEntity.ContactUsPageUrl;
            providerCourse.ContactUsPhoneNumber = patchedProviderCourseEntity.ContactUsPhoneNumber;
            providerCourse.StandardInfoUrl = patchedProviderCourseEntity.StandardInfoUrl;
            providerCourse.IsApprovedByRegulator = patchedProviderCourseEntity.IsApprovedByRegulator;

            await _roatpDataContext.SaveChangesAsync();

            return providerCourse;
        }

        public async Task<ProviderCourse> CreateProviderCourse(ProviderCourse providerCourse)
        {
            await _roatpDataContext.ProviderCourses.AddAsync(providerCourse);

            await _roatpDataContext.SaveChangesAsync();

            return providerCourse;
        }

        public async Task Delete(int ukprn, int larscode, string userId, string userDisplayName, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerCourse = await _roatpDataContext.ProviderCourses
                .Where(l => l.LarsCode == larscode && l.Provider.Ukprn == ukprn)
                .Include(l => l.Locations).Include(l => l.Versions)
                .SingleAsync();

                AddAudit(providerCourse, null, providerCourse.Id.ToString(), userId, userDisplayName, userAction);

                _roatpDataContext.Remove(providerCourse);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderCourse delete failed for ukprn {ukprn}, larscode {larscode} by userId {userId}", ukprn, larscode, userId);
                throw;
            }
        }
    }
}
