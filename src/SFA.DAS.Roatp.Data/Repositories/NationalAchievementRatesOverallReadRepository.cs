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
    internal class NationalAchievementRatesOverallReadRepository : INationalAchievementRatesOverallReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverallReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<NationalAchievementRateOverall>> GetBySectorSubjectArea(string expectedSectorSubjectArea)
        {
            var results = await _roatpDataContext.NationalAchievementRateOverall.Where(c =>
                    c.SectorSubjectArea.Equals(expectedSectorSubjectArea))
                .ToListAsync();

            return results;
        }
    }
}
