using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface ILoadProviderRepository
    {
        Task<bool> LoadProvider(Provider provider);
    }
}
