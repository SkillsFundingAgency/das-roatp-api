﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IGetActiveProviderRegistrationsRepository
    {
        Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations();
    }
}