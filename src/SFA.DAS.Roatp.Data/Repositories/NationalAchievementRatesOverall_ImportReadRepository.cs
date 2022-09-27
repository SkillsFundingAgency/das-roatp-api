using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRatesOverall_ImportReadRepository : INationalAchievementRatesOverall_ImportReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverall_ImportReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<NationalAchievementRateOverall_Import>> GetAllWithAchievementData()
        {
            var items = await _roatpDataContext.NationalAchievementRateOverallImports
                 .Where(c => c.OverallCohort.HasValue || c.OverallAchievementRate.HasValue)
                 .ToListAsync();
            return items;
        }
    }
}
