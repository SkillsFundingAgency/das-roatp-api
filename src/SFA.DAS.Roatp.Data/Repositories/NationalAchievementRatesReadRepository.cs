using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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
        var results = await _roatpDataContext.NationalAchievementRates.Where(c => c.Ukprn == ukprn).ToListAsync();

        return results;
    }

    public async Task<List<NationalAchievementRate>> GetAll()
    {
        return await _roatpDataContext.NationalAchievementRates.ToListAsync();
    }

    public async Task<List<NationalAchievementRate>> GetByProvidersLevelsSectorSubjectArea(List<int> ukprns, List<ApprenticeshipLevel> levels, int sectorSubjectAreaTier1)
    {
        return await _roatpDataContext.NationalAchievementRates
          .Where(x => (levels.Contains(x.ApprenticeshipLevel))
                        && x.Age == Age.AllAges
                        && x.SectorSubjectAreaTier1 == sectorSubjectAreaTier1
                        && ukprns.Contains(x.Ukprn))
          .ToListAsync();
    }
}