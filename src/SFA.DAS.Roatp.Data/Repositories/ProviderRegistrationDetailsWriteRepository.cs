using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderRegistrationDetailsWriteRepository : IProviderRegistrationDetailsWriteRepository
    {

        private readonly RoatpDataContext _roatpDataContext;
        public ProviderRegistrationDetailsWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<ProviderRegistrationDetail>> GetActiveProviders() =>
            await _roatpDataContext.ProviderRegistrationDetails
            .Where(x =>
                x.StatusId == (int)OrganisationStatus.Active ||
                x.StatusId == (int)OrganisationStatus.ActiveNotTakingOnApprentices ||
                x.StatusId == (int)OrganisationStatus.Onboarding)
            .ToListAsync();

        public async Task UpdateProviders(DateTime timeStarted, int providerCount, ImportType importType)
        {
            // since the entities were retrieved with tracking on, it is assumed that when the call is made the tracked entities are already updated
            // hence just need to add the audit entity and commit the changes here
            _roatpDataContext.ImportAudits.Add(new ImportAudit(timeStarted, providerCount, importType));
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task<ProviderRegistrationDetail> GetProviderRegistrationDetail(int ukprn)
        {
            return await _roatpDataContext.ProviderRegistrationDetails
                .FirstOrDefaultAsync(x => x.Ukprn == ukprn);
        }
    }
}
