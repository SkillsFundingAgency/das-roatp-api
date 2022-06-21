using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ICourseDirectoryDataProcessingService
    {
        public Task RemoveProvidersNotActiveOnRegister(List<CdProvider> providers);
        public Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers);
        public Task RemoveProvidersNotOnPilotList(List<CdProvider> providers);

        public Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(List<CdProvider> providers);
        public Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(List<CdProvider> providers);

        public Task InsertMissingPilotData(List<CdProvider> providers);
    }
}