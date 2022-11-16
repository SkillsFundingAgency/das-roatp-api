using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

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

    public async Task<List<NationalAchievementRate>> GetByProvidersLevelsSectorSubjectArea(List<int> providerIds, List<ApprenticeshipLevel> levels, string sectorSubjectArea)
    {
        return await _roatpDataContext.NationalAchievementRates
          .Where(x=>(levels.Contains(x.ApprenticeshipLevel)) 
                                            && x.Age==Age.AllAges 
                                            && x.SectorSubjectArea==sectorSubjectArea 
                                            && providerIds.Contains(x.ProviderId))  
          .ToListAsync();
    }
}