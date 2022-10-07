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
    public class LoadUkrlpAddressesService: ILoadUkrlpAddressesService
    {
        private readonly IProvidersReadRepository _providersReadRepository;
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly IReloadProviderAddressesRepository _providerAddressesRepository;
        private readonly IImportAuditWriteRepository _importAuditWriteRepository;
        private readonly ILogger<LoadUkrlpAddressesService> _logger;

        public LoadUkrlpAddressesService(IProvidersReadRepository providersReadRepository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IReloadProviderAddressesRepository providerAddressesRepository, IImportAuditWriteRepository importAuditWriteRepository, ILogger<LoadUkrlpAddressesService> logger)
        {
            _providersReadRepository = providersReadRepository;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _providerAddressesRepository = providerAddressesRepository;
            _importAuditWriteRepository = importAuditWriteRepository;
            _logger = logger;
        }

        public async Task<bool> LoadUkrlpAddresses()
        {
            var timeStarted = DateTime.UtcNow;
            var providers = await _providersReadRepository.GetAllProviders();
      
            var ukprnsSubset = providers.Select(provider => provider.Ukprn).Select(conv => (long)conv).ToList();

            var request = new ProviderAddressLookupRequest
            {
                Ukprns = ukprnsSubset
            };

            var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address",request);

            if (!success || !ukrlpResponse.Any())
            {
                _logger.LogError($"LoadAllProviderAddressesFunction function failed to get ukrlp addresses");
                return false;
            }

            foreach (var ukrlpProvider in ukrlpResponse)
            {
                ukrlpProvider.ProviderId = providers.First(x => x.Ukprn == ukrlpProvider.Ukprn).Id;
            }

            var providerAddresses = new List<ProviderAddress>();
            foreach (var ukrlpProvider in ukrlpResponse)
            {
                providerAddresses.Add(ukrlpProvider);
            }

            await _providerAddressesRepository.ReloadProviderAddresses(providerAddresses);

            _logger.LogInformation("Provider addresses reload complete");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddresses.Count, ImportType.ProviderAddresses));

            return true;

        }
    }
}

