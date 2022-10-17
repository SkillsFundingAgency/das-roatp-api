using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IReloadProviderAddressesRepository
{
    Task<bool> ReloadProviderAddresses(List<ProviderAddress> providerAddresses);
    Task<bool> UpdateProviderAddresses(List<ProviderAddress> providerAddresses);
}