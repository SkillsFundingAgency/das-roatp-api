using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

namespace SFA.DAS.Roatp.Jobs.Services;

public class ReloadProviderRegistrationDetailService : IReloadProviderRegistrationDetailService
{
    private readonly IReloadProviderRegistrationDetailsRepository _reloadProviderRegistrationDetailsRepository;
    private readonly IReloadProviderCourseTypesRepository _reloadProviderCourseTypesRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly ILogger<ReloadProviderRegistrationDetailService> _logger;
    private readonly IProviderRegistrationDetailsWriteRepository _providerRegistrationDetailsWriteRepository;
    private readonly IImportAuditReadRepository _importAuditReadRepository;

    public ReloadProviderRegistrationDetailService(
        IReloadProviderRegistrationDetailsRepository reloadProviderRegistrationDetailsRepository,
        ICourseManagementOuterApiClient courseManagementOuterApiClient,
        ILogger<ReloadProviderRegistrationDetailService> logger,
        IProviderRegistrationDetailsWriteRepository providerRegistrationDetailsWriteRepository,
        IReloadProviderCourseTypesRepository reloadProviderCourseTypesRepository,
        IImportAuditReadRepository importAuditReadRepository)
    {
        _reloadProviderRegistrationDetailsRepository = reloadProviderRegistrationDetailsRepository;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _logger = logger;
        _providerRegistrationDetailsWriteRepository = providerRegistrationDetailsWriteRepository;
        _reloadProviderCourseTypesRepository = reloadProviderCourseTypesRepository;
        _importAuditReadRepository = importAuditReadRepository;
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

        var providerCourseTypes = new List<ProviderCourseType>();

        foreach (var provider in providerRegistrationDetails)
        {
            providerCourseTypes.AddRange(provider.AllowedCourseTypes.Select(providerCourseType => new ProviderCourseType { Ukprn = provider.Ukprn, CourseType = providerCourseType.CourseTypeName }));
        }

        _logger.LogInformation("Reloading {Count} provider course types", providerCourseTypes.Count);
        await _reloadProviderCourseTypesRepository.ReloadProviderCourseTypes(providerCourseTypes, DateTime.Now);
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

    public async Task ReloadAllCoordinates()
    {
        var timeStarted = DateTime.UtcNow;
        var providers = await _providerRegistrationDetailsWriteRepository.GetActiveProviders();

        foreach (var provider in providers)
        {
            if (string.IsNullOrWhiteSpace(provider.Postcode))
            {
                _logger.LogWarning("Provider with Ukprn: {Ukprn} has no postcode", provider.Ukprn);
                continue;
            }

            var (success, lookupAddresses) = await _courseManagementOuterApiClient.Get<AddressList>($"lookup/addresses?postcode={provider.Postcode}");

            if (!success)
            {
                _logger.LogWarning("Attempt to get address for Ukprn:{Ukprn} Postcode: {Postcode} failed", provider.Ukprn, provider.Postcode);
                continue;
            }

            if (lookupAddresses.Addresses.Count == 0)
            {
                _logger.LogWarning("Attempt to get address for Ukprn:{Ukprn} Postcode: {Postcode} returned no addresses", provider.Ukprn, provider.Postcode);
                continue;
            }

            var firstAddress = lookupAddresses.Addresses[0];
            provider.Latitude = firstAddress.Latitude;
            provider.Longitude = firstAddress.Longitude;
        }

        await _providerRegistrationDetailsWriteRepository.UpdateProviders(timeStarted, providers.Count, ImportType.ProviderRegistrationAddressCoordinates);
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
