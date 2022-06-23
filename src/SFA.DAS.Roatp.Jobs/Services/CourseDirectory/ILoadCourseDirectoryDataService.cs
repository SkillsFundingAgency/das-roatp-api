using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public interface ILoadCourseDirectoryDataService
    {
         Task<CourseDirectoryImportMetrics> LoadCourseDirectoryData(bool betaAndPilotProvidersOnly);
    }
}
