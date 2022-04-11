using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Services
{
    public interface IProviderService
    {
        Task<Provider> GetProvider(int ukprn);
        Task<Provider> UpdateProvider(int ukprn, bool hasConfirmedDetails);
    }
}
