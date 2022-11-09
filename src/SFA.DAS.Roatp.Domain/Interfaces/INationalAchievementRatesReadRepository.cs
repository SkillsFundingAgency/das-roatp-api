using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface INationalAchievementRatesReadRepository
{
    Task<List<NationalAchievementRate>> GetByUkprn(int ukprn);
    Task<List<NationalAchievementRate>> GetAll();
}