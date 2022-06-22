using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class ImportCourseDirectoryDataService: IImportCourseDirectoryDataService
    {
        private readonly ILogger<ImportCourseDirectoryDataService> _logger;
        private readonly ILoadProviderRepository _loadProvider;

        public ImportCourseDirectoryDataService(ILogger<ImportCourseDirectoryDataService> logger, ILoadProviderRepository loadProvider)
        {
            _logger = logger;
            _loadProvider = loadProvider;
        }

        public async Task<CourseDirectoryImportMetrics> ImportProviders(Provider provider)
        {
            var metrics = new CourseDirectoryImportMetrics();
            
            var successfulLoading = await _loadProvider.LoadProvider(provider);
            if (successfulLoading)
            {
                _logger.LogInformation($"Ukprn {provider.Ukprn} mapped and loaded successfully");
                metrics.SuccessfulLoads = 1;
            }
            else
            {
                _logger.LogWarning($"Ukprn {provider.Ukprn} failed to load");
                metrics.FailedLoads = 1;
            }
                
            return metrics;
        }
    }
}