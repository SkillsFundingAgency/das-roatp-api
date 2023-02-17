using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ICourseDirectoryDataProcessingService
    {
        public Task<int> RemoveProvidersNotActiveOnRegister(List<CdProvider> providers);
        public Task<int> RemovePreviouslyLoadedProviders(List<CdProvider> providers);
        public Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(CdProvider provider);
        public Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(CdProvider provider, bool localRun);
        public Task AugmentPilotData(Provider provider);
        public Task<(bool, Provider)> MapCourseDirectoryProvider(CdProvider cdProvider, List<Standard> standards, List<Region> regions);
    }
}