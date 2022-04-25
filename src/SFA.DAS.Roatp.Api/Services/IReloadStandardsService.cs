using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Api.Models;

namespace SFA.DAS.Roatp.Api.Services
{
    public interface IReloadStandardsService
    {
        public Task<bool> ReloadStandards(List<Standard> standards);
    }
}