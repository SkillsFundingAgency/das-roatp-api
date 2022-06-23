using System;
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

            var responseMessage = string.Empty;

            // MFCMFC return body rather than logs

            if (loadMetrics.FailedLoads == 0 && loadMetrics.FailedMappings == 0 &&
                loadMetrics.LocationDuplicationMetrics.ProviderLocationsRemoved == 0 &&
                loadMetrics.LarsCodeDuplicationMetrics.ProviderStandardsRemoved == 0)
            {
                responseMessage =
                    $"Load providers from course directory beta providers only: [{betaAndPilotProvidersOnly}] successful with no issues" +
                    $": {loadMetrics.ProvidersToLoad} for loading, {loadMetrics.SuccessfulLoads} loaded";
            }
            else
            {
                responseMessage="Load providers from course directory did not fully load successfully: " +
                                $"{loadMetrics.SuccessfulLoads} providers successfully loaded (beta providers only: [{betaAndPilotProvidersOnly}]), {loadMetrics.FailedMappings} failed to map, " +
                                $"{loadMetrics.FailedLoads} failed to load, " +
                                $"{loadMetrics.LocationDuplicationMetrics.ProvidersWithDuplicateLocationNames} providers with duplicate locations removed, " +
                                $"{loadMetrics.LocationDuplicationMetrics.ProviderLocationsRemoved} total duplicate locations removed, " +
                                $"{loadMetrics.LarsCodeDuplicationMetrics.ProviderStandardsRemoved} providers with duplicate standards removed, " +
                                $"{loadMetrics.LarsCodeDuplicationMetrics.ProvidersWithDuplicateStandards} total duplicate standards removed. " +
                                "See individual logs for details";
            }

            log.LogInformation(responseMessage);
            return new OkObjectResult(loadMetrics);
        }
    }
}
