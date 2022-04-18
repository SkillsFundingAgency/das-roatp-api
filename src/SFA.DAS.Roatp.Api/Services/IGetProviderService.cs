using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Services
{
    public interface IGetProviderService
    {
        Task<Provider> GetProvider(int ukprn);
    }
}
