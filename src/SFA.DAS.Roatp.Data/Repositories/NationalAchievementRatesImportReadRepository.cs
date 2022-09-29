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
    internal class NationalAchievementRatesImportReadRepository : INationalAchievementRatesImportReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesImportReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<NationalAchievementRateImport>> GetAllWithAchievementData()
        {
            var items = await _roatpDataContext.NationalAchievementRateImports
                .Where(c => c.OverallCohort.HasValue || c.OverallAchievementRate.HasValue)
                .ToListAsync();
            return items;
        }
    }
}
