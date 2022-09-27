using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRatesOverallWriteRepository : INationalAchievementRatesOverallWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverallWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task DeleteAll()
        {
            _roatpDataContext.NationalAchievementRateOverall.RemoveRange(_roatpDataContext.NationalAchievementRateOverall);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task InsertMany(List<NationalAchievementRateOverall> items)
        {
            await _roatpDataContext.NationalAchievementRateOverall.AddRangeAsync(items);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
