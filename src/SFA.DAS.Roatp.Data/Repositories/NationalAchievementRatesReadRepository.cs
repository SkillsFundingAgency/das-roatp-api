using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class NationalAchievementRatesReadRepository : INationalAchievementRatesReadRepository
{
    private readonly RoatpDataContext _roatpDataContext;

    public NationalAchievementRatesReadRepository(RoatpDataContext roatpDataContext)
    {
        _roatpDataContext = roatpDataContext;
    }

    public async Task<List<NationalAchievementRate>> GetByUkprn(int ukprn)
    {
        var results = await _roatpDataContext.NationalAchievementRates.Where(c =>
            c.Provider.Ukprn == ukprn).ToListAsync();

        return results;
    }

    public async Task<List<NationalAchievementRate>> GetAll()
    {
        return await _roatpDataContext.NationalAchievementRates.ToListAsync();
    }
}