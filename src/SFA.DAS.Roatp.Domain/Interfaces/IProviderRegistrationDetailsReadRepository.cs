using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderRegistrationDetailsReadRepository
    {
        Task<List<ProviderRegistrationDetail>> GetActiveProviderRegistrations();
    }
}