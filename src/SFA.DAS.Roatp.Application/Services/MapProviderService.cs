using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using Provider = SFA.DAS.Roatp.Domain.ApiModels.Import.Provider;
using ProviderCourse = SFA.DAS.Roatp.Domain.Entities.ProviderCourse;

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

        public async Task<Domain.Entities.Provider> MapProvider(Provider provider)
        {
            var newProvider = new Domain.Entities.Provider
            {
                Ukprn = provider.Ukprn,
                LegalName = provider.LegalName,
                TradingName = provider.TradingName,
                Email = provider.Email,
                Website = provider.Website,
                Phone = provider.Phone,
                EmployerSatisfaction = provider.EmployerSatisfaction,
                LearnerSatisfaction = provider.LearnerSatisfaction,
                MarketingInfo = provider.MarketingInfo,
                HasConfirmedDetails = false,
                HasConfirmedLocations = false,
                Courses = new List<ProviderCourse>()
            };

            var standards = await _standardReadRepository.GetAllStandards();

            foreach (var providerCourse in provider.Standards)
            {

                var standard = standards.FirstOrDefault(x=>x.LarsCode== providerCourse.LarsCode);
                if (standard == null)
                {
                    _logger.LogWarning("No standard details were found for larscode {larscode}",providerCourse.LarsCode);
                    return null;
                }

                newProvider.Courses.Add( new ProviderCourse
                {
                    LarsCode = providerCourse.LarsCode,
                    StandardInfoUrl = providerCourse.StandardInfoUrl,
                    ContactUsPhoneNumber = providerCourse.ContactUsPhoneNumber,
                    ContactUsEmail = providerCourse.ContactUsEmail,
                    ContactUsPageUrl = providerCourse.ContactUsPageUrl,
                    IsImported = true,
                    IsConfirmed = false,
                    HasNationalDeliveryOption = false,
                    HasHundredPercentEmployerDeliveryOption = false,
                    Versions = new List<ProviderCourseVersion>
                    {
                        new ProviderCourseVersion
                        {
                            StandardUId = standard.StandardUId,
                            Version = standard.Version
                        }
                    }
                });
            }

            return newProvider;
        }
    }
}
