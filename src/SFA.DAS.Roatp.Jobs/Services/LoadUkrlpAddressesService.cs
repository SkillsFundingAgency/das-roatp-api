﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.SystemFunctions;
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

            var ukprnsSubset = providers.Select(provider => provider.Ukprn).ToList();
        

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

            var providerAddresses = new List<ProviderAddress>();
            foreach (var ukrlpProvider in ukrlpResponse)
            {
                var providerId = providers.FirstOrDefault(x => x.Ukprn == ukrlpProvider.Ukprn)?.Id;
                if (providerId != null)
                {
                        providerAddresses.Add(MapProviderAddress(ukrlpProvider, providerId.GetValueOrDefault()));
                    }
                    else
                {
                    _logger.LogInformation($"There was no matching ProviderId for ukprn {ukrlpProvider.Ukprn}, so this was not added to ProviderAddress");
                }
            }

            await _providerAddressesRepository.ReloadProviderAddresses(providerAddresses);

            _logger.LogInformation("Provider addresses reload complete");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddresses.Count, ImportType.ProviderAddresses));

            return true;
        }
        public async Task<bool> LoadUkrlpAddressesSinceLastUpdated()
        {
            var timeStarted = DateTime.UtcNow;
            var providers = await _providersReadRepository.GetAllProviders();

            var providersUpdatedSince = DateTime.Now;
            // get sinceLastUpdated from ImportAudit;


            var request = new ProviderAddressLookupRequest
            {
                Ukprns = new List<long>(),
                ProvidersUpdatedSince = providersUpdatedSince
            };

            var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request);

            if (!success || !ukrlpResponse.Any())
            {
                _logger.LogError($"LoadAllProviderAddressesFunction function failed to get ukrlp addresses");
                return false;
            }

            var providerAddresses = new List<ProviderAddress>();
            foreach (var ukrlpProvider in ukrlpResponse)
            {
                var providerId = providers.FirstOrDefault(x => x.Ukprn == ukrlpProvider.Ukprn)?.Id;
                if (providerId != null)
                    providerAddresses.Add(MapProviderAddress(ukrlpProvider, providerId.GetValueOrDefault()));
                else
                {
                    _logger.LogInformation($"There was no matching ProviderId for ukprn {ukrlpProvider.Ukprn}, so this was not added to ProviderAddress");
                }
            }

            await _providerAddressesRepository.UpdateProviderAddresses(providerAddresses);

            _logger.LogInformation("Provider addresses reload complete");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddresses.Count, ImportType.ProviderAddresses));

            return true;
        }

        private static ProviderAddress MapProviderAddress(UkrlpProviderAddress source, int providerId)
        {
        return new ProviderAddress
            {
                AddressLine1 = source.Address1,
                AddressLine2 = source.Address2,
                AddressLine3 = source.Address3,
                AddressLine4 = source.Address4,
                Town = source.Town,
                Postcode = source.Postcode,
                AddressUpdateDate = DateTime.Now,
                ProviderId = providerId
        };
        }
    }
}

