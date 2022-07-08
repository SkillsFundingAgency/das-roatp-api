using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class GetBetaProvidersService: IGetBetaProvidersService
    {
        private readonly ProviderFeaturesConfiguration _providerFeaturesConfiguration;

        public GetBetaProvidersService(ProviderFeaturesConfiguration providerFeaturesConfiguration)
        {
            _providerFeaturesConfiguration = providerFeaturesConfiguration;
        }

        public Task<List<int>> GetBetaProviderUkprns()
        {
            var featureToggles = _providerFeaturesConfiguration.FeatureToggles;

            var courseManagementFeature = featureToggles.First(x => x.Feature == "CourseManagement");
            if (courseManagementFeature?.Whitelist == null)
                return Task.FromResult(new List<int>());
            
            var ukprns = courseManagementFeature.Whitelist.Select(z => Convert.ToInt32(z.Ukprn)).ToList();

            return Task.FromResult(ukprns);
        }
    }
}
