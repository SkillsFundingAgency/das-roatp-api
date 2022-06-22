using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface ICourseDirectoryDataProcessingService
    {
        public Task RemoveProvidersNotActiveOnRegister(List<CdProvider> providers);
        public Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers);
        public Task RemoveProvidersNotOnBetaList(List<CdProvider> providers);
        public Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(CdProvider provider);
        public Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(CdProvider provider);
        public Task InsertMissingPilotData(CdProvider provider);
        public Task<(bool, Provider)> MapCourseDirectoryProvider(CdProvider cdProvider, List<Standard> standards, List<Region> regions);
    }
}