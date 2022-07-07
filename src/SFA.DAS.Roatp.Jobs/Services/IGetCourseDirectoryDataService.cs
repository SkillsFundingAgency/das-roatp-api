using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public interface IGetCourseDirectoryDataService
    {
        public Task<List<CdProvider>> GetCourseDirectoryData();
    }
}
