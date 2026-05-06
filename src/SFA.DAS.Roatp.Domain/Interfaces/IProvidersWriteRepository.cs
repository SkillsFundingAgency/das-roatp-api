using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProvidersWriteRepository
    {
        Task Patch(Provider patchedProviderEntity, string userId, string userDisplayName, string userAction);
        Task<Provider> Create(Provider provider, string userId, string userDisplayName, string userAction);
        Task<List<Provider>> GetAllProviders();
        Task UpdateProviders(DateTime timeStarted, int providerCount, ImportType importType);
    }
}

