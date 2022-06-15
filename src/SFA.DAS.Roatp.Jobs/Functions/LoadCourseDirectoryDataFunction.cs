using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Roatp.Jobs.Services.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadCourseDirectoryDataFunction
    {
        private readonly ILoadCourseDirectoryDataService _loadCourseDirectoryDataService;

        public LoadCourseDirectoryDataFunction(ILoadCourseDirectoryDataService loadCourseDirectoryDataService)
        {
            _loadCourseDirectoryDataService = loadCourseDirectoryDataService;
        }

        // http trigger
        [FunctionName(nameof(LoadCourseDirectoryDataFunction))]
        public async Task Run([TimerTrigger("%ReloadStandardsCacheSchedule%"
#if DEBUG
            , RunOnStartup=true
#endif
        
        )] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation("LoadCourseDirectoryData function started");

            await _loadCourseDirectoryDataService.LoadCourseDirectoryData();

            log.LogInformation("Course directory data complete");
        }
    }
}
