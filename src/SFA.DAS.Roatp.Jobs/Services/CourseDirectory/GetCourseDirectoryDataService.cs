using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class GetCourseDirectoryDataService : IGetCourseDirectoryDataService
    {
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<GetCourseDirectoryDataService> _logger;

        public GetCourseDirectoryDataService(ICourseManagementOuterApiClient courseManagementOuterApiClient,
            ILogger<GetCourseDirectoryDataService> logger)
        {
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _logger = logger;
        }

        public async Task<List<CdProvider>> GetCourseDirectoryData()
        {
            var (success, courseDirectoryResponse) = await _courseManagementOuterApiClient.Get<string>("lookup/course-directory-data");

            if (!success)
            {
                const string errorMessage = "GetCourseDirectoryData: Unexpected response when trying to get course directory details from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            var cdProviders = JsonConvert.DeserializeObject<List<CdProvider>>(courseDirectoryResponse);
            _logger.LogInformation("GetCourseDirectoryData: {count} providers returned from Course Directory", cdProviders.Count);

            return cdProviders;
        }
    }
}
