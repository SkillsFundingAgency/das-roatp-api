using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    [ExcludeFromCodeCoverage]
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
                const string errorMessage = "LoadUkrlpAddresses: Unexpected response when trying to get course directory details from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            var sizeOfResponse = courseDirectoryResponse.Length;
            var sizeInMegabytes = ((sizeOfResponse / 1024F) / 1024F).ToString("0.00");
            _logger.LogInformation("Course Directory Date size: {sizeOfResponse} Mb",sizeInMegabytes);
            var cdProviders = JsonConvert.DeserializeObject<List<CdProvider>>(courseDirectoryResponse);
            _logger.LogInformation("LoadUkrlpAddresses: {count} providers returned from Course Directory", cdProviders.Count);

            return cdProviders;
        }
    }
}
