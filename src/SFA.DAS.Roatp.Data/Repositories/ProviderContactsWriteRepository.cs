using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
public class ProviderContactsWriteRepository : IProviderContactsWriteRepository
{
    private readonly RoatpDataContext _roatpDataContext;
    private readonly ILogger<ProviderContactsWriteRepository> _logger;
    public ProviderContactsWriteRepository(RoatpDataContext context, ILogger<ProviderContactsWriteRepository> logger)
    {
        _roatpDataContext = context;
        _logger = logger;
    }

    public async Task<ProviderContact> CreateProviderContact(ProviderContact providerContact, int ukprn, string userId, string userDisplayName, List<int> providerCourseIds)
    {
        var strategy = _roatpDataContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                var providerCourses = await _roatpDataContext
                    .ProviderCourses
                    .Where(c => c.Provider.Ukprn == ukprn && providerCourseIds.Contains(c.Id))
                    .ToListAsync();

                await _roatpDataContext.ProviderContacts.AddAsync(providerContact);
                Audit audit = new(nameof(ProviderContact), ukprn.ToString(), userId, userDisplayName, AuditEventTypes.CreateProviderContact, providerContact, null);
                _roatpDataContext.Audits.Add(audit);


                foreach (var providerCourse in providerCourses)
                {
                    ProviderCourse providerCourseOriginal = JsonSerializer.Deserialize<ProviderCourse>(JsonSerializer.Serialize(providerCourse));

                    if (!string.IsNullOrEmpty(providerContact.EmailAddress))
                    {
                        providerCourse.ContactUsEmail = providerContact.EmailAddress;
                    }

                    if (!string.IsNullOrEmpty(providerContact.PhoneNumber))
                    {
                        providerCourse.ContactUsPhoneNumber = providerContact.PhoneNumber;
                    }

                    Audit auditProviderCourse = new(nameof(ProviderCourse), ukprn.ToString(), userId, userDisplayName,
                        AuditEventTypes.UpdateProviderCourse, providerCourseOriginal, providerCourse);
                    _roatpDataContext.Audits.Add(auditProviderCourse);
                }

                await _roatpDataContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderContact Create failed for ukprn {Ukprn} by userId {UserId}", ukprn, userId);
                throw;
            }
        });

        return providerContact;
    }
}