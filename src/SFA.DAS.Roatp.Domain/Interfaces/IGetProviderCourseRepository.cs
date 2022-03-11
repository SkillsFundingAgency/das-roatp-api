using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IGetProviderCourseRepository
    {
        Task<ProviderCourse> GetProviderCourse(int ukprn, int larsCode);
        Task<Provider> GetAllProviderCourse(int ukprn);
    }
}