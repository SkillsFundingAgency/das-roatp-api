using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Requests;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadProviderRegistrationDetailService : IReloadProviderRegistrationDetailService
    {
        private readonly IProviderRegistrationDetailsWriteRepository _providerRegistrationDetailsWriteRepository;
        private readonly IReloadProviderRegistrationDetailsRepository _reloadProviderRegistrationDetailsRepository;
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<ReloadProviderRegistrationDetailService> _logger;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;
        public ReloadProviderRegistrationDetailService(IReloadProviderRegistrationDetailsRepository reloadProviderRegistrationDetailsRepository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IImportAuditWriteRepository importAuditWriteRepository, ILogger<ReloadProviderRegistrationDetailService> logger, IProviderRegistrationDetailsWriteRepository providerRegistrationDetailsWriteRepository)
        {
            _reloadProviderRegistrationDetailsRepository = reloadProviderRegistrationDetailsRepository;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _importAuditWriteRepository = importAuditWriteRepository;
            _logger = logger;
            _providerRegistrationDetailsWriteRepository = providerRegistrationDetailsWriteRepository;
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
            await _reloadProviderRegistrationDetailsRepository.ReloadRegisteredProviders(providerRegistrationDetails);
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerRegistrationDetails.Count, ImportType.ProviderRegistrationDetails));
        }

        public async Task ReloadAllAddresses()
        {
            var timeStarted = DateTime.UtcNow;
            var activeProvidersOnRegister = await _providerRegistrationDetailsWriteRepository.GetActiveProviders();

            var ukprnsSubset = activeProvidersOnRegister.Select(provider => provider.Ukprn).ToList();

            var request = new ProviderAddressLookupRequest
            {
                Ukprns = ukprnsSubset
            };

            var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request);

            if (!success || !ukrlpResponse.Any())
            {
                _logger.LogError($"LoadAllProviderAddressesFunction function failed to get ukrlp addresses");
                return;
            }

            foreach (var activeProvider in activeProvidersOnRegister)
            {
                var ukrlpProvider = ukrlpResponse.FirstOrDefault(x => x.Ukprn == activeProvider.Ukprn);
                if (ukrlpProvider == null)
                {
                    _logger.LogWarning($"Unable to get address from UKRLP for provider ukprn: {activeProvider.Ukprn}");
                    continue;

                }
                activeProvider.UpdateAddress(ukrlpProvider);
            }

            await _providerRegistrationDetailsWriteRepository.UpdateProviders(timeStarted, ukrlpResponse.Count);

            _logger.LogInformation("Provider registration addresses reload complete");
        }
    }
}
