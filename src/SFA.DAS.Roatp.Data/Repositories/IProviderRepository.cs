using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public interface IProviderRepository
    {
        Task<Provider> GetProviderByUkprn(int ukprn);
    }
}