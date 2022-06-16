using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class CourseDirectoryDataProcessingService : ICourseDirectoryDataProcessingService
    {
        private readonly IGetActiveProviderRegistrationsRepository _getActiveProviderRegistrationsRepository;
        private readonly IProviderReadRepository _providerReadRepository;
        private readonly ILogger<CourseDirectoryDataProcessingService> _logger;

        public CourseDirectoryDataProcessingService(ILogger<CourseDirectoryDataProcessingService> logger, IGetActiveProviderRegistrationsRepository getActiveProviderRegistrationsRepository, IProviderReadRepository providerReadRepository)
        {
            _getActiveProviderRegistrationsRepository = getActiveProviderRegistrationsRepository;
            _providerReadRepository = providerReadRepository;
            _logger = logger;
        }

        public async Task RemoveProvidersNotActiveOnRegister(List<CdProvider> providers)
        {
            var focusText = "active registered providers from roatp-service cache";
            _logger.LogInformation($"Gathering {focusText}");
            var activeProviders = await _getActiveProviderRegistrationsRepository.GetActiveProviderRegistrations();
            _logger.LogInformation($"{activeProviders.Count} {focusText}");
            _logger.LogInformation($"{providers.Count} CD providers before removing non-{focusText}");
           
            providers.RemoveAll(x => !activeProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers after removing non-{focusText}");

        }

        public async Task RemoveProvidersAlreadyPresentOnRoatp(List<CdProvider> providers)
        {
            var focusText = "providers already present in roatp database";
            _logger.LogInformation($"Gathering {focusText}");
            var currentProviders = await _providerReadRepository.GetAllProviders();
            _logger.LogInformation($"{currentProviders.Count} {focusText}");
            _logger.LogInformation($"{providers.Count} CD providers before removing {focusText}");

            providers.RemoveAll(x => currentProviders.Select(x => x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers to insert after removing {focusText}");
        }
    }
}