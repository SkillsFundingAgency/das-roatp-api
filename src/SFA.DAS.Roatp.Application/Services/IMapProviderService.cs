using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.Services
{
    public interface IMapProviderService
    {
        public Task<Provider> MapProvider(CdProvider cdProvider);
    }
}