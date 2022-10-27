
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IProviderDetailsReadRepository
    {
        Task<ProviderDetailsWithDistance> GetProviderDetailsWithDistance(int ukprn, double? lat, double? lon);
    }
}
