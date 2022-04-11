using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderRepository
    {
        Task<Provider> GetProvider(int ukprn);
        Task<Provider> UpdateProvider(int ukprn, bool hasConfirmedDetails);
    }
}