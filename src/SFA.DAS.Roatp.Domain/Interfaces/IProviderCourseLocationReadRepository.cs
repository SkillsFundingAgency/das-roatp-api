using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationReadRepository
    {
        Task<List<ProviderCourseLocation>> GetAllProviderCourseLocations(int ukprn, int larsCode);
    }
}