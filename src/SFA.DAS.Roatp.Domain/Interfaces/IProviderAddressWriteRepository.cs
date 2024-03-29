﻿using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IProviderAddressWriteRepository
{
    Task<bool> Update(ProviderAddress providerAddress);
}