using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRates_ImportWriteRepository : INationalAchievementRates_ImportWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRates_ImportWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task DeleteAll()
        {
            _roatpDataContext.NationalAchievementRateImports.RemoveRange(_roatpDataContext.NationalAchievementRateImports);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task InsertMany(List<NationalAchievementRate_Import> items)
        {
            await _roatpDataContext.NationalAchievementRateImports.AddRangeAsync(items);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
