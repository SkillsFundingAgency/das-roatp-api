using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ReloadProviderRegistrationDetailService : IReloadProviderRegistrationDetailService
{
    private readonly IReloadProviderRegistrationDetailsRepository _reloadProviderRegistrationDetailsRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly ILogger<ReloadProviderRegistrationDetailService> _logger;
    private readonly IProviderRegistrationDetailsWriteRepository _providerRegistrationDetailsWriteRepository;

    public ReloadProviderRegistrationDetailService(
        IReloadProviderRegistrationDetailsRepository reloadProviderRegistrationDetailsRepository,
        ICourseManagementOuterApiClient courseManagementOuterApiClient,
        ILogger<ReloadProviderRegistrationDetailService> logger,
        IProviderRegistrationDetailsWriteRepository providerRegistrationDetailsWriteRepository)
    {
        _reloadProviderRegistrationDetailsRepository = reloadProviderRegistrationDetailsRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _logger = logger;
        _providerRegistrationDetailsWriteRepository = providerRegistrationDetailsWriteRepository;
    }

    public async Task ReloadProviderRegistrationDetails()
    {
        var timeStarted = DateTime.UtcNow;

        var (success, providerRegistrationDetails) = await _courseManagementOuterApiClient.Get<List<RegisteredProviderModel>>("lookup/registered-providers");

        if (!success)
        {
            const string errorMessage = "Unexpected response when trying to get provider registration details from the outer api.";
            _logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        _logger.LogInformation("Reloading {Count} provider registration details", providerRegistrationDetails.Count);
        List<ProviderRegistrationDetail> registeredProviders = providerRegistrationDetails.Select(prd => (ProviderRegistrationDetail)prd).ToList();

        await _reloadProviderRegistrationDetailsRepository.ReloadRegisteredProviders(registeredProviders, timeStarted);
    }

    public async Task ReloadAllAddresses()
    {
        var timeStarted = DateTime.UtcNow;
        var activeProvidersOnRegister = await _providerRegistrationDetailsWriteRepository.GetActiveProviders();
        var ukprns = activeProvidersOnRegister.Select(provider => provider.Ukprn).ToList();

        var request = new GetUkrlpProvidersRequest(ukprns, null);

        var (success, ukrlpResponse) = await _courseManagementOuterApiClient.Post<GetUkrlpProvidersRequest, GetUkrlpProvidersResponse>(Constants.GetUkrlpDataRequestUrl, request);

        if (!success)
        {
            _logger.LogError("LoadAllProviderAddressesFunction function failed to get ukrlp addresses");
            return;
        }

        foreach (var ukrlpProvider in ukrlpResponse.Providers)
        {
            _logger.LogInformation("Updating address for provider ukprn: {Ukprn} with latest ukrlp info", ukrlpProvider.Ukprn);
            var activeProvider = activeProvidersOnRegister.Single(x => x.Ukprn == ukrlpProvider.Ukprn);
            UpdateAddress(activeProvider, ukrlpProvider.LegalAddress);
        }

        await _providerRegistrationDetailsWriteRepository.UpdateProviders(timeStarted, ukrlpResponse.Providers.Count(), ImportType.ProviderRegistrationAddresses);

        _logger.LogInformation("Provider registration addresses reload complete");
    }

    private static void UpdateAddress(ProviderRegistrationDetail provider, Address source)
    {
        provider.AddressLine1 = source.Address1;
        provider.AddressLine2 = source.Address2;
        provider.AddressLine3 = source.Address3;
        provider.AddressLine4 = source.Address4;
        provider.Town = source.Town;
        provider.Postcode = source.Postcode;
    }
}
