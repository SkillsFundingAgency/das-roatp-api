using SFA.DAS.Roatp.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderCourseLocationWriteRepository
    {
        Task<ProviderCourseLocation> Create(ProviderCourseLocation providerCourseLocation);
    }
}
