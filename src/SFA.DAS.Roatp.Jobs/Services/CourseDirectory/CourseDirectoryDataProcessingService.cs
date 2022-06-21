using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services.Metrics;

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

        public async Task RemoveProvidersNotOnPilotList(List<CdProvider> providers)
        {
            var focusText = "pilot providers";
           
            _logger.LogInformation($"{providers.Count} CD providers before removing non-{focusText}");
            providers.RemoveAll(x => !PilotProviders.Pilots.Select(x=>x.Ukprn).Contains(x.Ukprn));
            _logger.LogInformation($"{providers.Count} CD providers to insert after removing non-{focusText}");
        }

        public async Task<LocationDuplicationMetrics> CleanseDuplicateLocationNames(List<CdProvider> providers)
        {
            var metrics = new LocationDuplicationMetrics();

            foreach (var provider in providers)
            {
                var currentLocationNames = new List<string>();
                var locationsToRemove = new List<CdProviderLocation>();
                foreach (var location in provider.Locations)
                {
                    if (!currentLocationNames.Contains(location.Name))
                    {
                        currentLocationNames.Add(location.Name);
                    }
                    else
                    {
                        locationsToRemove.Add(location);
                    }
                }

                if (locationsToRemove.Any())
                {
                    metrics.ProvidersWithDuplicateNames++;
                    foreach (var locationToRemove in locationsToRemove)
                    {
                        provider.Locations.Remove(locationToRemove);
                        metrics.ProviderLocationsRemoved++;
                        _logger.LogWarning($"Duplicate location name - provider UKPRN {provider.Ukprn}: removing location id {locationToRemove.Id} location name '{locationToRemove.Name}'");
                    }
                }
            }

            return metrics;
        }

        public async Task<LarsCodeDuplicationMetrics> CleanseDuplicateLarsCodes(List<CdProvider> providers)
        {
            var metrics = new LarsCodeDuplicationMetrics();

            foreach (var provider in providers)
            {
                var currentLarsCodes = new List<int>();
                var coursesToRemove = new List<CdProviderCourse>();
                foreach (var course in provider.Standards)
                {
                    if (!currentLarsCodes.Contains(course.StandardCode))
                    {
                        currentLarsCodes.Add(course.StandardCode);
                    }
                    else
                    {
                        coursesToRemove.Add(course);
                    }
                }

                if (coursesToRemove.Any())
                {
                    metrics.ProvidersWithDuplicateStandards++;
                    foreach (var courseToRemove in coursesToRemove)
                    {
                        provider.Standards.Remove(courseToRemove);
                        metrics.ProviderStandardsRemoved++;
                        _logger.LogWarning($"Duplicate lars code - provider UKPRN {provider.Ukprn}: removing duplicate larsCode {courseToRemove.StandardCode}'");
                    }
                }
            }

            return metrics;
        }

        public async Task InsertMissingPilotData(List<CdProvider> providers)
        {
            foreach (var pilot in PilotProviders.Pilots.Where(pilot => providers.All(p => p.Ukprn != pilot.Ukprn)))
            {
                providers.Add(new CdProvider {Name = pilot.Name, Ukprn = pilot.Ukprn});
                _logger.LogInformation($"Add pilot provider for provider UKPRN {pilot.Ukprn}");
            }


            foreach (var provider in providers)
            {
                foreach (var pilotCourse in PilotProviderCourses.PilotCourses.Where(pilotCourse => provider.Standards.All(x => x.StandardCode != pilotCourse.LarsCode)))
                {
                    provider.Standards.Add(new CdProviderCourse {StandardCode = pilotCourse.LarsCode, StandardInfoUrl = pilotCourse.StandardInfoUrl});
                    _logger.LogInformation($"Adding pilot courses for UKPRN {provider.Ukprn} LarsCode {pilotCourse.LarsCode}");
                }
            }
        }
    }
}