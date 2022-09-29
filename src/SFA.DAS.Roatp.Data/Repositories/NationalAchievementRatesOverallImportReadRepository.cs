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
    internal class NationalAchievementRatesOverallImportReadRepository : INationalAchievementRatesOverallImportReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverallImportReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<NationalAchievementRateOverallImport>> GetAllWithAchievementData()
        {
            var items = await _roatpDataContext.NationalAchievementRateOverallImports
                 .Where(c => c.OverallCohort.HasValue || c.OverallAchievementRate.HasValue)
                 .ToListAsync();
            return items;
        }
    }
}
