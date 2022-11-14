﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProviderDetailsReadRepository : IProviderDetailsReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;
        private readonly ILogger<ProviderDetailsReadRepository> _logger;

        public ProviderDetailsReadRepository(ILogger<ProviderDetailsReadRepository> logger, RoatpDataContext roatpDataContext)
        {
            _logger = logger;
            _roatpDataContext = roatpDataContext;
        }

        public async Task<ProviderCourseDetailsModel> GetProviderDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon)
        {
            _logger.LogInformation("Gathering ProviderDetails with distance for ukprn {ukprn}, larscode {larscode}", ukprn,larsCode); 
            var provider = await _roatpDataContext.ProviderDetailsWithDistance
                .FromSqlInterpolated(GetProvidersDetailsWithDistanceSql(ukprn, larsCode, lat, lon)).FirstOrDefaultAsync();
            return provider;
        }

        public async Task<List<ProviderCourseLocationDetailsModel>> GetProviderlocationDetailsWithDistance(int ukprn, int larsCode, double? lat, double? lon)
        {
            _logger.LogInformation("Gathering ProviderLocationDetails with distance for ukprn {ukprn}, larscode {larscode}", ukprn, larsCode);

            var providerLocations = await _roatpDataContext.ProviderLocationDetailsWithDistance
                .FromSqlInterpolated(GetProviderLocationDetailsWithDistanceSql(ukprn, larsCode, lat, lon)).ToListAsync();
            return providerLocations;
        }

        private static FormattableString GetProvidersDetailsWithDistanceSql(int ukprn, int larsCode, double? lat, double? lon )
        {
            return $@"
                   select p.ukprn,
                            pc.LarsCode,
                            p.LegalName,
                            p.TradingName,
                            p.MarketingInfo,
		                    pc.StandardInfoUrl,
		                    pc.ContactUsEmail as Email,
		                    pc.ContactUsPhoneNumber as Phone,
		                    pc.ContactUsPageUrl as StandardContactUrl,
                            p.Website as ProviderWebsite,
                            pa.AddressLine1 as Address1,
                            pa.AddressLine2 as Address2,
                            pa.AddressLine3 as Address3,
                            pa.AddressLine4 as Address4,
                            pa.Town as Town,
                            PA.Postcode as Postcode,
                            PA.Latitude,
                            PA.Longitude,
                            CASE  WHEN ({lat} is null) THEN null
                                WHEN ({lon} is null) THEN null
                                ELSE
                                    geography::Point(isnull(pa.Latitude,0), isnull(pa.Longitude,0), 4326)
                                            .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
			                                as Distance
                            FROM provider P
		                    INNER JOIN ProviderCourse pc on p.Id = pc.ProviderId
							LEFT OUTER JOIN [dbo].[ProviderAddress] PA on P.Id = PA.ProviderId
		                    Where P.ukprn={ukprn}
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
	                CASE	WHEN ({lat} is null) THEN null
			                WHEN ({lon} is null) THEN null
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