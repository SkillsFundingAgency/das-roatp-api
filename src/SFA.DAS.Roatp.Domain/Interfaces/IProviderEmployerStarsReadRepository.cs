using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderEmployerStarsReadRepository
{
    Task<List<string>> GetTimePeriods();
}