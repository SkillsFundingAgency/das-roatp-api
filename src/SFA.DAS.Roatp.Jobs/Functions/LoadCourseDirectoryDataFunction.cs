using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.Services.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Functions
{
    public class LoadCourseDirectoryDataFunction
    {
        private readonly ILoadCourseDirectoryDataService _loadCourseDirectoryDataService;

        [ExcludeFromCodeCoverage]
        public LoadCourseDirectoryDataFunction(ILoadCourseDirectoryDataService loadCourseDirectoryDataService)
        {
            _loadCourseDirectoryDataService = loadCourseDirectoryDataService;
        }

        [FunctionName(nameof(LoadCourseDirectoryDataFunction))]
        public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function,  "POST", Route = "load-course-directory")] HttpRequest req, ILogger log)
        
        {
            var betaAndPilotProvidersOnlyParameter = req.Query["betaAndPilotOnly"];

            var betaAndPilotProvidersOnly = true;
            if (betaAndPilotProvidersOnlyParameter.Count > 0 && bool.TryParse(betaAndPilotProvidersOnlyParameter[0], out var betaAndPilotProvidersOnlyValue))
            {
                betaAndPilotProvidersOnly = betaAndPilotProvidersOnlyValue;
            }

            log.LogInformation("LoadCourseDirectoryDataFunction started");

            var loadMetrics = await _loadCourseDirectoryDataService.LoadCourseDirectoryData(betaAndPilotProvidersOnly);

            log.LogInformation("Course Directory load complete");
            return new OkObjectResult(loadMetrics);
        }
    }
}
