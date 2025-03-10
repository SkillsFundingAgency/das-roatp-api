using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderRegistrationDetailsReadRepository : IProviderRegistrationDetailsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderRegistrationDetailsReadRepository> _logger;

        public ProviderRegistrationDetailsReadRepository(RoatpDataContext roatpDataContext, ILogger<ProviderRegistrationDetailsReadRepository> logger)
        {
            _roatpDataContext = roatpDataContext;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        public async Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations(CancellationToken cancellationToken)
        {
            var activeProviders = await _roatpDataContext.ProviderRegistrationDetails
                .Where(x =>
                        x.StatusId == OrganisationStatus.Active ||
                        x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices ||
                        x.StatusId == OrganisationStatus.Onboarding)
                .Include(r => r.Provider)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} active provider registration details from ProviderRegistrationDetail", activeProviders.Count);

            return activeProviders;
        }

        public async Task<List<ProviderRegistrationDetail>> GetActiveAndMainProviderRegistrations(CancellationToken cancellationToken)
        {
            var distinctUkrpns = await (from pr1 in _roatpDataContext.Providers
                              join tp in _roatpDataContext.ProviderRegistrationDetails on pr1.Ukprn equals tp.Ukprn
                              join pc1 in _roatpDataContext.ProviderCourses on pr1.Id equals pc1.ProviderId
                              join pl1 in _roatpDataContext.ProviderLocations on pr1.Id equals pl1.ProviderId
                         where tp.StatusId == 1 && tp.ProviderTypeId == 1
                         select pr1.Ukprn
             )
             .Distinct()
             .ToListAsync(cancellationToken);

            return await _roatpDataContext.ProviderRegistrationDetails
                .Where(a => distinctUkrpns.Contains(a.Ukprn)).ToListAsync(cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        public async Task<ProviderRegistrationDetail> GetProviderRegistrationDetail(int ukprn)
            => await _roatpDataContext
                    .ProviderRegistrationDetails
                    .Where(x =>
                        x.StatusId == OrganisationStatus.Active ||
                        x.StatusId == OrganisationStatus.ActiveNotTakingOnApprentices ||
                        x.StatusId == OrganisationStatus.Onboarding)
                    .Include(r => r.Provider)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Ukprn == ukprn);

        [ExcludeFromCodeCoverage]
        public async Task<bool> IsMainActiveProvider(int ukprn, int larsCode)
        {
            var count = (await _roatpDataContext.Database.SqlQueryRaw<int>(@"
                            SELECT COUNT(*) as cnt
                            FROM [dbo].[ProviderCourse] pc1
                            JOIN [dbo].[Provider] pr1 ON pr1.Id = pc1.ProviderId 
                            JOIN [dbo].[ProviderRegistrationDetail] tp 
                                ON tp.[Ukprn] = pr1.[Ukprn] AND tp.[StatusId] = 1 AND tp.[ProviderTypeId] = 1 
                            JOIN [dbo].[ProviderCourseLocation] pcl1 ON pcl1.ProviderCourseId = pc1.[Id]
                            JOIN [dbo].[ProviderLocation] pl1 ON pl1.Id = pcl1.ProviderLocationId
                            JOIN [dbo].[Standard] sd1 ON sd1.LarsCode = pc1.[LarsCode]
                            WHERE pc1.[LarsCode] = @larsCode 
                            AND pr1.[Ukprn] = @ukprn",
                            new SqlParameter("@larsCode", larsCode),
                            new SqlParameter("@ukprn", ukprn))
                        .ToListAsync())[0];

            return count > 0;
        }

    }
}
