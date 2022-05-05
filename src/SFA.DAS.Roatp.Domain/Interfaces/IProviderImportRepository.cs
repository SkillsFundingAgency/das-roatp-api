using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderImportRepository
    {
        public Task<bool> ImportProviderDetails(Provider provider);
    }
}
