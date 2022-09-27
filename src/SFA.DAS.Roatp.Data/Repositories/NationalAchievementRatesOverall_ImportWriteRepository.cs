using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class NationalAchievementRatesOverall_ImportWriteRepository : INationalAchievementRatesOverall_ImportWriteRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public NationalAchievementRatesOverall_ImportWriteRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task DeleteAll()
        {
            _roatpDataContext.NationalAchievementRateOverallImports.RemoveRange(_roatpDataContext.NationalAchievementRateOverallImports);
            await _roatpDataContext.SaveChangesAsync();
        }

        public async Task InsertMany(List<NationalAchievementRateOverall_Import> items)
        {
            await _roatpDataContext.NationalAchievementRateOverallImports.AddRangeAsync(items);
            await _roatpDataContext.SaveChangesAsync();
        }
    }
}
