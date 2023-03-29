using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Data.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderRegistrationDetailsWriteRepository : IProviderRegistrationDetailsWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderRegistrationDetailsWriteRepository> _logger;
        public ProviderRegistrationDetailsWriteRepository(RoatpDataContext roatpDataContext, ILogger<ProviderRegistrationDetailsWriteRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<List<ProviderRegistrationDetail>> GetActiveProviders() =>
            await _roatpDataContext.ProviderRegistrationDetails
            .Where(x =>
                x.StatusId == OrganisationStatus.Active ||
                x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices ||
                x.StatusId == OrganisationStatus.Onboarding)
            .ToListAsync();

        public async Task UpdateProviders(DateTime timeStarted, int providerCount, ImportType importType)
        {
            // since the entities were retrieved with tracking on, it is assumed that when the call is made the tracked entities are already updated
            // hence just need to add the audit entity and commit the changes here
            _roatpDataContext.ImportAudits.Add(new ImportAudit(timeStarted, providerCount, importType));
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task<ProviderRegistrationDetail> Create(ProviderRegistrationDetail providerRegistrationDetail, string userId, string userDisplayName, string userAction)
        {
            await using var transaction = await _roatpDataContext.Database.BeginTransactionAsync();
            try
            {
                _roatpDataContext.ProviderRegistrationDetails.Add(providerRegistrationDetail);
            
                Audit audit = new(nameof(ProviderRegistrationDetail), providerRegistrationDetail.Ukprn.ToString(), userId, userDisplayName, userAction, providerRegistrationDetail, null);
            
                _roatpDataContext.Audits.Add(audit);
            
                await _roatpDataContext.SaveChangesAsync();
            
                await transaction.CommitAsync();
                return providerRegistrationDetail;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "ProviderRegistrationDetail create is failed for ukprn {ukprn} by userId {userId}", providerRegistrationDetail.Ukprn, userId);
                throw;
            }
        }
    }
}
