using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IReloadNationalAcheivementRatesLookupService
    {
        Task ReloadNationalAcheivementRates();
    }
}