using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRatesOverallReadRepository : INationalAchievementRatesOverallReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverallReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<NationalAchievementRateOverall>> GetBySectorSubjectArea(int sectorSubjectAreaTier1Code)
        {
            var results = await _roatpDataContext.NationalAchievementRateOverall.Where(c => c.SectorSubjectAreaTier1 == sectorSubjectAreaTier1Code).ToListAsync();

            return results;
        }
    }
}
