using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

public class RestrictedCourseWriteRepository(RoatpDataContext _roatpDataContext, ILogger<RestrictedCourseWriteRepository> _logger) : IRestrictedCourseWriteRepository
{
    public async Task CreateRestrictedCourse(string larsCode, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();

            try
            {
                var restrictedCourse = new RestrictedCourse
                {
                    LarsCode = larsCode,
                    CreatedDate = DateTime.UtcNow
                };

                var providerAllowedCourses = await (
                from providerCourse in _roatpDataContext.ProviderCourses
                join provider in _roatpDataContext.Providers
                    on providerCourse.ProviderId equals provider.Id
                where providerCourse.LarsCode == larsCode
                      && !_roatpDataContext.ProviderAllowedCourses.Any(pac =>
                          pac.Ukprn == provider.Ukprn &&
                          pac.LarsCode == larsCode)
                select new ProviderAllowedCourse
                {
                    Ukprn = provider.Ukprn,
                    LarsCode = larsCode
                })
                .Distinct()
                .ToListAsync();

                await _roatpDataContext.RestrictedCourses.AddAsync(restrictedCourse);

                await _roatpDataContext.ProviderAllowedCourses.AddRangeAsync(providerAllowedCourses);

                var initialState = new
                {
                    AllowedProviders = providerAllowedCourses
                    .Select(x => x.Ukprn)
                    .Distinct()
                    .ToList()
                };

                var audit = new Audit(
                    nameof(RestrictedCourse),
                    larsCode,
                    userId,
                    userDisplayName,
                    userAction,
                    initialState,
                    null);

                _roatpDataContext.Audits.Add(audit);

                await _roatpDataContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "RestrictedCourse create failed for LarsCode {LarsCode} by userId {UserId}", larsCode, userId);
                throw;
            }
        });
    }
}
