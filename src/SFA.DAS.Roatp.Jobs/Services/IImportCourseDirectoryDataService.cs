using System.Threading.Tasks;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IImportCourseDirectoryDataService
    {
        public Task<CourseDirectoryImportMetrics> ImportProviders(Provider provider);
    }
}