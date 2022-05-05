using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Entities;
using Provider = SFA.DAS.Roatp.Domain.ApiModels.Import.Provider;

namespace SFA.DAS.Roatp.Application.Services
{
    public interface IMapProviderService
    {
        public Task<Domain.Entities.Provider> MapProvider(Provider provider);
    }
}