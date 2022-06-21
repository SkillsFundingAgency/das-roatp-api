using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IImportCourseDirectoryDataService
    {
        public Task<CourseDirectoryImportMetrics> ImportCourseDirectoryData(List<CdProvider> cdProviders);
        public Task<(bool, Provider)> MapCourseDirectoryProvider(CdProvider cdProvider, List<Standard> standards, List<Region> regions);
    }
}