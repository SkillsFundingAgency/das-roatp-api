using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IReloadProviderRegistrationDetailsRepository
    {
        Task<bool> ReloadRegisteredProviders(List<ProviderRegistrationDetail> providerRegistrationDetails, DateTime timeStarted);
    }
}
