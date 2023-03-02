using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IReloadProviderRegistrationDetailService
    {
        Task ReloadProviderRegistrationDetails();
        Task ReloadAllAddresses();
        Task ReloadAllCoordinates();
    }
}