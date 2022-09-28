using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.Services
{
    public class ReloadNationalAcheivementRatesLookupService : IReloadNationalAcheivementRatesLookupService
    {
        private readonly ICourseManagementOuterApiClient _courseManagementOuterApiClient;
        private readonly ILogger<ReloadNationalAcheivementRatesLookupService> _logger;
        private readonly IReloadNationalAcheivementRatesService _reloadNationalAcheivementRatesService;
        private readonly IReloadNationalAcheivementRatesOverallService _reloadNationalAcheivementRatesOverallService;

        public ReloadNationalAcheivementRatesLookupService(
                ILogger<ReloadNationalAcheivementRatesLookupService> logger,
                ICourseManagementOuterApiClient courseManagementOuterApiClient,
                IReloadNationalAcheivementRatesService reloadNationalAcheivementRatesService,
                IReloadNationalAcheivementRatesOverallService reloadNationalAcheivementRatesOverallService
                )
        {
            _logger = logger;
            _courseManagementOuterApiClient = courseManagementOuterApiClient;
            _reloadNationalAcheivementRatesService = reloadNationalAcheivementRatesService;
            _reloadNationalAcheivementRatesOverallService = reloadNationalAcheivementRatesOverallService;
        }

        public async Task ReloadNationalAcheivementRates()
        {
            var (success, nationalAchievementRatesLookup) = await _courseManagementOuterApiClient.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates");
            if (!success)
            {
                const string errorMessage = "Unexpected response when trying to get national achievement rates from the outer api.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            await _reloadNationalAcheivementRatesService.ReloadNationalAcheivementRates(nationalAchievementRatesLookup.NationalAchievementRates);

            await _reloadNationalAcheivementRatesOverallService.ReloadNationalAcheivementRatesOverall(nationalAchievementRatesLookup.OverallAchievementRates);
        }
    }
}
