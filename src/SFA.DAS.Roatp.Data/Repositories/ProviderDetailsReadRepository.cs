using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    public class ProviderDetailsReadRepository : IProviderDetailsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderDetailsReadRepository> _logger;

        public ProviderDetailsReadRepository(ILogger<ProviderDetailsReadRepository> logger, RoatpDataContext roatpDataContext)
        {
            _logger = logger;
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderAndCourseDetailsWithDistance> GetProviderDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon)
        {
            var provider = await _roatpDataContext.ProviderDetailsWithDistance
                .FromSqlInterpolated(GetProvidersDetailsWithDistanceSql(ukprn, larsCode, lat, lon)).FirstOrDefaultAsync();
            return provider;
        }

        public async Task<List<ProviderLocationDetailsWithDistance>> GetProviderlocationDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon)
        {
            var providerLocations = await _roatpDataContext.ProviderLocationDetailsWithDistance
                .FromSqlInterpolated(GetProviderLocationDetailsWithDistanceSql(ukprn, larsCode, lat, lon)).ToListAsync();
            return providerLocations;
        }

        private static FormattableString GetProvidersDetailsWithDistanceSql(int ukprn, int larsCode, double? lat, double? lon )
        {
            return $@"
                    select pdv.ukprn,
                            pc.LarsCode,
                            pdv.LegalName,
                            pdv.TradingName,
                            pdv.MarketingInfo,
		                    pc.StandardInfoUrl,
		                    pc.ContactUsEmail as Email,
                            pc.ContactUsPhoneNumber as Phone,
		                    pc.ContactUsPageUrl as StandardContactUrl,
                            pdv.Website as ProviderWebsite,
                            pdv.AddressLine1 as Address1,
                            pdv.AddressLine2 as Address2,
                            pdv.AddressLine3 as Address3,
                            pdv.AddressLine4 as Address4,
                            pdv.Town as Town,
                            Pdv.Postcode as Postcode,
                            Pdv.Latitude,
                            Pdv.Longitude,
                            CASE  WHEN ({lat} is null) THEN 0
                                WHEN ({lon} is null) THEN 0
                                ELSE
                                    geography::Point(isnull(pdv.Latitude,0), isnull(pdv.Longitude,0), 4326)
                                            .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
			                                as Distance
                            FROM providerDetailsView pdv
		                    INNER JOIN ProviderCourse pc on pdv.Id = pc.ProviderId
		                    Where ukprn={ukprn}
		                    AND pc.LarsCode={larsCode}";
        }

        private static FormattableString GetProviderLocationDetailsWithDistanceSql(int ukprn, int larsCode, double? lat, double? lon)
        {
            return $@"
                    SELECT P.Ukprn,
                    PC.LarsCode,
	                LocationName,
                    PL.Email,
                    PL.Website,
                    PL.Phone,
	                LocationType,
	                PL.ImportedLocationId as LocationId,
	                PCL.HasDayReleaseDeliveryOption,
	                PCL.HasBlockReleaseDeliveryOption,
	                AddressLine1,
	                AddressLine2,
	                Town,
	                Postcode,
	                R.RegionName,
	                R.SubregionName,
	                PL.Latitude,
	                PL.Longitude,
	                CASE	WHEN ({lat} is null) THEN 0
			                WHEN ({lon} is null) THEN 0
	                ELSE
	                  geography::Point(isnull(PL.Latitude,0), isnull(PL.Longitude,0), 4326)
				                .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
				                as Distance
                      FROM Provider P
                      INNER JOIN ProviderCourse PC
                      ON p.Id = PC.ProviderID
                      INNER JOIN ProviderCourseLocation PCL on PC.Id = PCL.ProviderCourseId
                      INNER JOIN ProviderLocation PL On PCL.ProviderLocationId = PL.Id
                      LEFT OUTER JOIN Region R on R.Id =PL.RegionId
                      WHERE P.Ukprn={ukprn}
                      AND LarsCode={larsCode}";
        }
    }
}
