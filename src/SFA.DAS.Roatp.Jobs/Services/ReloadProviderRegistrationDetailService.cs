using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadProviderRegistrationDetailService : IReloadProviderRegistrationDetailService
    {
        private readonly IReloadProviderRegistrationDetailsRepository _repository;
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<ReloadProviderRegistrationDetailService> _logger;

        public ReloadProviderRegistrationDetailService(IReloadProviderRegistrationDetailsRepository repository, ICourseManagementOuterApiClient courseManagementOuterApiClient, ILogger<ReloadProviderRegistrationDetailService> logger)
        {
            _repository = repository;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _logger = logger;
        }

        public async Task ReloadProviderRegistrationDetails()
        {
            var (success, providerRegistrationDetails) = await _courseManagementOuterApiClient.Get<List<ProviderRegistrationDetail>>("lookup/registered-providers");
            if (!success)
            {
                const string errorMessage = "Unexpected response when trying to get provider registration details from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            _logger.LogInformation($"Reloading {providerRegistrationDetails.Count} provider registration details");
            await _repository.ReloadRegisteredProviders(providerRegistrationDetails);
        }
    }
}
