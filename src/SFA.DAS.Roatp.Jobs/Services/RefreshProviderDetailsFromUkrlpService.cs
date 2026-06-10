using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;

namespace SFA.DAS.Roatp.Jobs.Services;

public class RefreshProviderDetailsFromUkrlpService(ICourseManagementOuterApiClient _courseManagementOuterApiClient, IImportAuditReadRepository _importAuditReadRepository, IProvidersWriteRepository _providersWriteRepository, ILogger<RefreshProviderDetailsFromUkrlpService> _logger) : IRefreshProviderDetailsFromUkrlpService
{
    public async Task RefreshProviderDetailsFromUkrlp(bool recentUpdatesOnly)
    {
        var timeStarted = DateTime.UtcNow;

        DateTime? updatedSinceDate = null;
        if (recentUpdatesOnly)
        {
            updatedSinceDate = await _importAuditReadRepository.GetLastImportedDateByImportType(ImportType.Providers);
        }

        var providers = await _providersWriteRepository.GetAllProviders();

        GetUkrlpProvidersRequest request = new(providers.Select(p => p.Ukprn), updatedSinceDate);
        var (success, response) = await _courseManagementOuterApiClient.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, request);
        if (!success)
        {
            _logger.LogWarning("Unable to get provider details from UKRLP, so provider details will not be updated");
            return;
        }

        if (!response.Providers.Any())
        {
            _logger.LogInformation("No provider details were returned from UKRLP, so provider details will not be updated");
            return;
        }

        var count = 0;
        foreach (var provider in response.Providers)
        {
            var existingProvider = providers.FirstOrDefault(p => p.Ukprn == provider.Ukprn);

            if (existingProvider != null)
            {
                count++;
                _logger.LogInformation("Updating existing provider with UKPRN {Ukprn}", provider.Ukprn);
                UpdateProviderDetails(provider, existingProvider);
            }
        }
        await _providersWriteRepository.UpdateProviders(timeStarted, count, ImportType.Providers);
    }

    private static void UpdateProviderDetails(ProviderDetails provider, Provider existingProvider)
    {
        existingProvider.LegalName = provider.LegalName;
        existingProvider.TradingName = provider.TradingName;
        existingProvider.Email = provider.PrimaryContact.Email;
        existingProvider.Phone = provider.PrimaryContact.Telephone;
        existingProvider.Website = provider.PrimaryContact.Website;

        if (provider.LegalAddress != null)
        {
            existingProvider.ProviderAddress ??= new();
            existingProvider.ProviderAddress.AddressLine1 = provider.LegalAddress.Address1;
            existingProvider.ProviderAddress.AddressLine2 = provider.LegalAddress.Address2;
            existingProvider.ProviderAddress.AddressLine3 = provider.LegalAddress.Address3;
            existingProvider.ProviderAddress.AddressLine4 = provider.LegalAddress.Address4;
            existingProvider.ProviderAddress.Town = provider.LegalAddress.Town;
            existingProvider.ProviderAddress.Postcode = provider.LegalAddress.Postcode;
            // set lat and lon to null so these can be refreshed by UpdateProviderAddressCoordinatesFunction
            existingProvider.ProviderAddress.Latitude = null;
            existingProvider.ProviderAddress.Longitude = null;
            existingProvider.ProviderAddress.AddressUpdateDate = DateTime.UtcNow;
        }
    }
}
