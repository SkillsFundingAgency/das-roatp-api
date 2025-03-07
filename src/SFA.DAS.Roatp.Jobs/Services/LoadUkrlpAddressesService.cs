﻿using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.Requests;

namespace SFA.DAS.Roatp.Jobs.Services;

public class LoadUkrlpAddressesService : ILoadUkrlpAddressesService
{
    private readonly IProvidersReadRepository _providersReadRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly IReloadProviderAddressesRepository _providerAddressesRepository;
    private readonly IImportAuditWriteRepository _importAuditWriteRepository;
    private readonly IImportAuditReadRepository _importAuditReadRepository;
    private readonly ILogger<LoadUkrlpAddressesService> _logger;

    public LoadUkrlpAddressesService(IProvidersReadRepository providersReadRepository, ICourseManagementOuterApiClient courseManagementOuterApiClient, IReloadProviderAddressesRepository providerAddressesRepository, IImportAuditWriteRepository importAuditWriteRepository, IImportAuditReadRepository importAuditReadRepository, ILogger<LoadUkrlpAddressesService> logger)
    {
        _providersReadRepository = providersReadRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _providerAddressesRepository = providerAddressesRepository;
        _importAuditWriteRepository = importAuditWriteRepository;
        _importAuditReadRepository = importAuditReadRepository;
        _logger = logger;
    }

    public async Task<bool> LoadAllProvidersAddresses()
    {
        var timeStarted = DateTime.UtcNow;
        var providers = await _providersReadRepository.GetAllProviders();

        var ukprnsSubset = providers.Select(provider => provider.Ukprn).ToList();

        var request = new ProviderAddressLookupRequest
        {
            Ukprns = ukprnsSubset
        };

        var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request);

        if (!success || ukrlpResponse.Count == 0)
        {
            _logger.LogError("LoadAllProviderAddressesFunction function failed to get ukrlp addresses");
            return false;
        }

        var providerAddresses = new List<ProviderAddress>();
        foreach (var ukrlpProvider in ukrlpResponse)
        {
            var providerId = providers.Find(x => x.Ukprn == ukrlpProvider.Ukprn)?.Id;
            if (providerId != null)
            {
                providerAddresses.Add(MapProviderAddress(ukrlpProvider, providerId.GetValueOrDefault()));
            }
            else
            {
                _logger.LogInformation("There was no matching ProviderId for ukprn {Ukprn}, so this was not added to ProviderAddress", ukrlpProvider.Ukprn);
            }
        }

        await _providerAddressesRepository.ReloadProviderAddresses(providerAddresses);

        _logger.LogInformation("Provider addresses reload complete");
        await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddresses.Count, ImportType.ProviderAddresses));

        return true;
    }

    public async Task<bool> LoadProvidersAddresses()
    {
        var timeStarted = DateTime.UtcNow;

        var providersUpdatedSince =
            await _importAuditReadRepository.GetLastImportedDateByImportType(ImportType.ProviderAddresses);

        var request = new ProviderAddressLookupRequest
        {
            Ukprns = new List<int>(),
            ProvidersUpdatedSince = providersUpdatedSince
        };

        var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<ProviderAddressLookupRequest, List<UkrlpProviderAddress>>("lookup/providers-address", request);

        if (!success || ukrlpResponse.Count == 0)
        {
            _logger.LogError("LoadProviderAddressesFunction function failed to get ukrlp addresses");
            return false;
        }

        _logger.LogInformation("LoadProviderAddressesFunction function returned {Count} ukrlp addresses", ukrlpResponse.Count);
        var providers = await _providersReadRepository.GetAllProviders();


        var providerAddresses = new List<ProviderAddress>();
        foreach (var ukrlpProvider in ukrlpResponse)
        {
            var providerId = providers.Find(x => x.Ukprn == ukrlpProvider.Ukprn)?.Id;
            if (providerId != null)
            {
                providerAddresses.Add(MapProviderAddress(ukrlpProvider, providerId.GetValueOrDefault()));
                _logger.LogInformation("There is a matching ProviderId for ukprn {Ukprn}, so this was added to ProviderAddresses to process", ukrlpProvider.Ukprn);
            }
            else
            {
                _logger.LogInformation("There was no matching ProviderId for ukprn {Ukprn}, so this was not added to ProviderAddresses to process", ukrlpProvider.Ukprn);
            }
        }

        if (providerAddresses.Count == 0)
        {
            _logger.LogInformation("No providers to update from the ProviderAddress upsert");
            return true;
        }

        _logger.LogInformation("{Count} providers found to update from the ProviderAddress upsert", providerAddresses.Count);

        var successfulUpsert = await _providerAddressesRepository.UpsertProviderAddresses(providerAddresses);

        if (successfulUpsert)
        {

            _logger.LogInformation("Provider addresses update based on ProvidersUpdatedSince complete");
            await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddresses.Count,
                ImportType.ProviderAddresses));
            return true;
        }

        _logger.LogWarning("Provider addresses update based on ProvidersUpdatedSince failed");
        return false;
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

