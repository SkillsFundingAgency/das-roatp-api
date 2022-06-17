using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Data.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class GetActiveProviderRegistrationsRepository: IGetActiveProviderRegistrationsRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<GetActiveProviderRegistrationsRepository> _logger;

        public GetActiveProviderRegistrationsRepository(RoatpDataContext roatpDataContext, ILogger<GetActiveProviderRegistrationsRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        public async Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations()
        {
            var providerRegistrationDetails =  await _roatpDataContext.ProviderRegistrationDetails.AsNoTracking().ToListAsync();
            _logger.LogInformation($"Retrieved {providerRegistrationDetails.Count()} provider registration details from ProviderRegistrationDetail");

            var activeProviders = providerRegistrationDetails.Where(x =>
                x.StatusId == OrganisationStatus.Active ||
                x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices).ToList();

            _logger.LogInformation($"Finding {activeProviders.Count} active providers");

            return activeProviders.ToList();
        }
    }
}
