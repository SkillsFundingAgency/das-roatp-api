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
        private readonly IImportAuditInsertRepository _importAuditInsertRepository;
        public ReloadProviderRegistrationDetailService(IReloadProviderRegistrationDetailsRepository repository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IImportAuditInsertRepository importAuditInsertRepository, ILogger<ReloadProviderRegistrationDetailService> logger)
        {
            _repository = repository;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _importAuditInsertRepository = importAuditInsertRepository;
            _logger = logger;
        }

        public async Task ReloadProviderRegistrationDetails()
        {
            var timeStarted = DateTime.UtcNow;
            var (success, providerRegistrationDetails) = await _courseManagementOuterApiClient.Get<List<ProviderRegistrationDetail>>("lookup/registered-providers");
            if (!success)
            {
                const string errorMessage = "Unexpected response when trying to get provider registration details from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            _logger.LogInformation($"Reloading {providerRegistrationDetails.Count} provider registration details");
            await _repository.ReloadRegisteredProviders(providerRegistrationDetails);
            await _importAuditInsertRepository.Insert(new ImportAudit(timeStarted, providerRegistrationDetails !=null? providerRegistrationDetails.Count:0, ImportType.ProviderRegistrationDetails));
            _logger.LogInformation("Provider Registration Details audit record written");
        }
    }
}
