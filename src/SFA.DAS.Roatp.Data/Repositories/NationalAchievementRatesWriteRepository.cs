using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRatesWriteRepository : INationalAchievementRatesWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task DeleteAll()
        {
            _roatpDataContext.NationalAchievementRates.RemoveRange(_roatpDataContext.NationalAchievementRates);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task InsertMany(List<NationalAchievementRate> items)
        {
            await _roatpDataContext.NationalAchievementRates.AddRangeAsync(items);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
