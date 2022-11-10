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
    public class ProviderRegistrationDetailsReadRepository: IProviderRegistrationDetailsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderRegistrationDetailsReadRepository> _logger;

        public ProviderRegistrationDetailsReadRepository(RoatpDataContext roatpDataContext, ILogger<ProviderRegistrationDetailsReadRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations()
        {
            var activeProviders =  await _roatpDataContext.ProviderRegistrationDetails.Where(x =>
                                                x.StatusId == OrganisationStatus.Active || 
                                                x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices ||
                                                x.StatusId == OrganisationStatus.Onboarding).AsNoTracking().ToListAsync();
            
            _logger.LogInformation("Retrieved {count} active provider registration details from ProviderRegistrationDetail", activeProviders.Count);

            return activeProviders.ToList();
        }
    }
}
