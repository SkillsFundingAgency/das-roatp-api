using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ICourseDirectoryDataProcessingService
    {
        public Task RemoveProvidersNotActiveOnRegister(List<CdProvider> providers);
        public Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers);
    }
}