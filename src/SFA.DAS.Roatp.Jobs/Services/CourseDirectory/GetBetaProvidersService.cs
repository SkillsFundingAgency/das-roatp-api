using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Authorization.ProviderFeatures.Configuration;

namespace SFA.DAS.Roatp.Jobs.Services.CourseDirectory
{
    public class GetBetaProvidersService: IGetBetaProvidersService
    {
        private const string CourseManagement = "CourseManagement";
        private readonly ProviderFeaturesConfiguration _providerFeaturesConfiguration;

        public GetBetaProvidersService(ProviderFeaturesConfiguration providerFeaturesConfiguration)
        {
            _providerFeaturesConfiguration = providerFeaturesConfiguration;
        }

        public List<int> GetBetaProviderUkprns()
        {
            var featureToggles = _providerFeaturesConfiguration.FeatureToggles;

            var courseManagementFeature = featureToggles.First(f => f.Feature == CourseManagement);
            return courseManagementFeature?.Whitelist == null ? 
                new List<int>() : 
                courseManagementFeature.Whitelist.Select(w => Convert.ToInt32(w.Ukprn)).ToList();
        }
    }
}
