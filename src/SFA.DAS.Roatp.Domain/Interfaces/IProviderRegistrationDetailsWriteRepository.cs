using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderRegistrationDetailsWriteRepository
    {
        Task<List<ProviderRegistrationDetail>> GetActiveProviders();
        Task UpdateProviders(DateTime timeStarted, int providerCount, ImportType importType);
      }
}