using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ILoadUkrlpAddressesService
    { 
        public Task<bool> LoadUkrlpAddresses();
    }
}
