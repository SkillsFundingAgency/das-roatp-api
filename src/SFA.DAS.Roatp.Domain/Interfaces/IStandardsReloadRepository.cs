﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IStandardsReloadRepository
    {
        Task<bool> ReloadStandards(List<Standard> standards);
    }
}