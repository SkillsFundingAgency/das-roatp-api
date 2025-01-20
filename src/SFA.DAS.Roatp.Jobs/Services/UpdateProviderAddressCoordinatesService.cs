using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;

namespace SFA.DAS.Roatp.Jobs.Services;

public class UpdateProviderAddressCoordinatesService : IUpdateProviderAddressCoordinatesService
{
    private readonly IProviderAddressReadRepository _providerAddressReadRepository;
    private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
    private readonly IProviderAddressWriteRepository _providerAddressWriteRepository;
    private readonly IImportAuditWriteRepository _importAuditWriteRepository;
    private readonly ILogger<UpdateProviderAddressCoordinatesService> _logger;

    public UpdateProviderAddressCoordinatesService(ILogger<UpdateProviderAddressCoordinatesService> logger, ICourseManagementOuterApiClient courseManagementOuterApiClient, IProviderAddressReadRepository providerAddressReadRepository, IProviderAddressWriteRepository providerAddressWriteRepository, IImportAuditWriteRepository importAuditWriteRepository)
    {
        _logger = logger;
        _courseManagementOuterApiClient = courseManagementOuterApiClient;
        _providerAddressReadRepository = providerAddressReadRepository;
        _providerAddressWriteRepository = providerAddressWriteRepository;
        _importAuditWriteRepository = importAuditWriteRepository;
    }

    public async Task UpdateProviderAddressCoordinates()
    {
        var noOfFailures = 0;
        var timeStarted = DateTime.UtcNow;
        var providerAddressesToProcess = await _providerAddressReadRepository.GetProviderAddressesWithMissingLatLongs();

        foreach (var address in providerAddressesToProcess)
        {
            if (address.Postcode == null)
            {
                _logger.LogInformation("ProviderAddress Id: {AddressId} has no postcode", address.Id);
                noOfFailures++;
                continue;
            }

            var (success, lookupAddresses) = await _courseManagementOuterApiClient.Get<AddressList>($"lookup/addresses?postcode={address.Postcode}");

            if (!success)
            {
                _logger.LogWarning("Attempt to get address for  postcode {Postcode} failed", address.Postcode);
                noOfFailures++;
                continue;
            }

            if (!lookupAddresses.Addresses.Any())
            {
                _logger.LogWarning("Attempt to get address for  postcode {Postcode} returned no addresses", address.Postcode);
                noOfFailures++;
                continue;
            }

            var firstAddress = lookupAddresses.Addresses[0];
            address.Latitude = firstAddress.Latitude;
            address.Longitude = firstAddress.Longitude;
            address.CoordinatesUpdateDate = DateTime.Now;

            var successfulUpdate = await _providerAddressWriteRepository.Update(address);

            if (!successfulUpdate)
                noOfFailures++;
        }
        if (noOfFailures == 0)
            _logger.LogInformation("ProviderAddress coordinates update for {Count} records has completed, with no failures", providerAddressesToProcess.Count);
        else
        {
            _logger.LogWarning("ProviderAddress coordinates update for {Count} records has completed, with {NoOfFailures} failure(s)", providerAddressesToProcess.Count, noOfFailures);
        }

        await _importAuditWriteRepository.Insert(new ImportAudit(timeStarted, providerAddressesToProcess.Count - noOfFailures,
            ImportType.ProviderAddressesLatLong));
    }
}