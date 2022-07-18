using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Domain.Interfaces
{
    public interface IGetStandardsCountRepository
    {
        Task<int> GetStandardsCount();
    }
}