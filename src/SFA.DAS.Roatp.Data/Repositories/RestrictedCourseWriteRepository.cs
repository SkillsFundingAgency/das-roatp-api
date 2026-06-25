using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]

public class RestrictedCourseWriteRepository(RoatpDataContext _roatpDataContext, ILogger<RestrictedCourseWriteRepository> _logger) : IRestrictedCourseWriteRepository
{
    public async Task CreateRestrictedCourse(string larsCode, RestrictedCourse restrictedCourse, string userId, string userDisplayName, string userAction)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();

            try
            {
                await _roatpDataContext.RestrictedCourses.AddAsync(restrictedCourse);

                var initialState = new
                {
                    AllowedProviders = restrictedCourse.ProviderAllowedCourses
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
                throw new InvalidOperationException();
            }
        });
    }
}
