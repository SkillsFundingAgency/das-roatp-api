using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface INationalAchievementRatesReadRepository
{
    Task<List<NationalAchievementRate>> GetByUkprn(int ukprn);
    Task<List<NationalAchievementRate>> GetAll();
    Task<List<NationalAchievementRate>> GetByProvidersLevelsSectorSubjectArea(List<int> providerIds, List<ApprenticeshipLevel> levels,
        string sectorSubjectArea);
}