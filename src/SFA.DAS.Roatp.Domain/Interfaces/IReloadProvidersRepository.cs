using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces;

public interface IReloadProvidersRepository
{
    Task<bool> ReloadProviders(DateTime timeStarted, List<Provider> providers);
}
