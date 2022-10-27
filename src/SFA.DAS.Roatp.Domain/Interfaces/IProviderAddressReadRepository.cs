using SFA.DAS.Roatp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderAddressReadRepository
    {
        Task<List<ProviderAddress>> GetAllProviderAddresses();
        Task<List<ProviderAddress>> GetProviderAddressesWithMissingLatLongs();
    }
}
