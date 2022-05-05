using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Services
{
    public class MapProviderService: IMapProviderService
    {
        private readonly IStandardReadRepository _standardReadRepository;
        private readonly ILogger<MapProviderService> _logger;

        public MapProviderService(IStandardReadRepository standardReadRepository, ILogger<MapProviderService> logger)
        {
            _standardReadRepository = standardReadRepository;
            _logger = logger;
        }

        public async Task<Provider> MapProvider(CdProvider cdProvider)
        {
            var provider = new Provider
            {
                Ukprn = cdProvider.Ukprn,
                LegalName = cdProvider.LegalName,
                TradingName = cdProvider.TradingName,
                Email = cdProvider.Email,
                Website = cdProvider.Website,
                Phone = cdProvider.Phone,
                EmployerSatisfaction = cdProvider.EmployerSatisfaction,
                LearnerSatisfaction = cdProvider.LearnerSatisfaction,
                MarketingInfo = cdProvider.MarketingInfo,
                HasConfirmedDetails = false,
                HasConfirmedLocations = false
            };

            var providerCourses = new List<ProviderCourse>();

            foreach (var cdProviderCourse in cdProvider.Courses)
            {

                var standard = await _standardReadRepository.GetStandard(cdProviderCourse.LarsCode);
                if (standard == null)
                {
                    _logger.LogWarning("No standard details were found for larscode {larscode}",cdProviderCourse.LarsCode);
                }

                var newProviderCourse = new ProviderCourse
                {
                    LarsCode = cdProviderCourse.LarsCode,
                    StandardInfoUrl = cdProviderCourse.StandardInfoUrl,
                    ContactUsPhoneNumber = cdProviderCourse.ContactUsPhoneNumber,
                    ContactUsEmail = cdProviderCourse.ContactUsEmail,
                    ContactUsPageUrl = cdProviderCourse.ContactUsPageUrl,
                    IsImported = true,
                    IsConfirmed = false,
                    HasNationalDeliveryOption = false,
                    HasHundredPercentEmployerDeliveryOption = false,
                    Versions = new List<ProviderCourseVersion>
                    {
                        new ProviderCourseVersion
                        {
                            StandardUId = standard?.StandardUId,
                            Version = standard?.Version
                        }
                    }
                };

                providerCourses.Add(newProviderCourse);
            }

            provider.Courses = providerCourses;
            return provider;

        }
    }
}
