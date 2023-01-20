using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Data.Constants;
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

        public async Task<ProviderCourseDetailsModel> GetProviderForUkprnAndLarsCodeWithDistance(int ukprn, int larsCode, decimal? latitude, decimal? longitude)
        {
            _logger.LogInformation("Gathering ProviderDetails with distance for ukprn {ukprn}, larscode {larscode}", ukprn,larsCode); 
            var provider = await _roatpDataContext.ProviderDetailsWithDistance
                .FromSqlInterpolated(GetProviderForUkprnAndLarsCodeWithDistanceSql(ukprn, larsCode, latitude, longitude)).FirstOrDefaultAsync();
            return provider;
        }

        public async Task<List<ProviderCourseLocationDetailsModel>> GetProviderLocationDetailsWithDistance(int ukprn, int larsCode, decimal? latitude, decimal? longitude)
        {
            _logger.LogInformation("Gathering ProviderLocationDetails with distance for ukprn {ukprn}, larscode {larscode}", ukprn, larsCode);

            var providerLocations = await _roatpDataContext.ProviderLocationDetailsWithDistance
                .FromSqlInterpolated(GetProviderLocationDetailsWithDistanceSql(ukprn, larsCode, latitude, longitude)).ToListAsync();
            return providerLocations;
        }

        public async Task<List<ProviderCourseSummaryModel>> GetProvidersForLarsCodeWithDistance(int larsCode, decimal? latitude, decimal? longitude)
        {
            _logger.LogInformation("Gathering all ProviderDetails with distance for larscode {larscode}",  larsCode);
            var providers = await _roatpDataContext.ProviderSummaryDetailsWithDistance
                .FromSqlInterpolated(GetProvidersForLarsCodeWithDistanceSql( larsCode, latitude, longitude)).ToListAsync();
            return providers;
        }

        public async Task<List<ProviderCourseLocationDetailsModel>> GetAllProviderlocationDetailsWithDistance(int larsCode,
            decimal? latitude, decimal? longitude)
        {
            _logger.LogInformation(
                "Gathering all ProviderLocationDetails with distance for larscode {larscode}", larsCode);

            var providerLocations = await _roatpDataContext.ProviderLocationDetailsWithDistance
                .FromSqlInterpolated(GetAllProviderLocationDetailsWithDistanceSql( larsCode, latitude, longitude))
                .ToListAsync();
            return providerLocations;

        }

        private static FormattableString GetProviderForUkprnAndLarsCodeWithDistanceSql(int ukprn, int larsCode, decimal? lat, decimal? lon )
        {
            return $@"
                   select   p.ukprn,
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
                            p.Id as ProviderId,
                            CASE  WHEN ({lat} is null) THEN null
                                WHEN ({lon} is null) THEN null
                                ELSE
                                    geography::Point(isnull(pa.Latitude,0), isnull(pa.Longitude,0), 4326)
                                            .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
			                                as Distance
                            FROM provider P
		                    INNER JOIN ProviderCourse pc on p.Id = pc.ProviderId
							LEFT OUTER JOIN [ProviderAddress] PA on P.Id = PA.ProviderId
                            LEFT OUTER JOIN ProviderRegistrationDetail PRD on P.Ukprn = PRD.Ukprn 
		                    Where P.ukprn={ukprn}
		                    AND pc.LarsCode={larsCode}
                            AND PRD.StatusId in ({OrganisationStatus.Active}, {OrganisationStatus.ActiveNotTakingOnApprentices})";
        }

        private static FormattableString GetProvidersForLarsCodeWithDistanceSql(int larsCode, decimal? lat, decimal? lon)
        {
            return $@"
                   select p.Id as ProviderId,
                            p.ukprn,
                            pc.LarsCode,
                            p.LegalName,
                            p.TradingName,
                            CASE  WHEN ({lat} is null) THEN null
                                WHEN ({lon} is null) THEN null
                                ELSE
                                    geography::Point(isnull(pa.Latitude,0), isnull(pa.Longitude,0), 4326)
                                            .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
			                                as Distance
                            FROM provider P
		                    INNER JOIN ProviderCourse pc on p.Id = pc.ProviderId
							LEFT OUTER JOIN [ProviderAddress] PA on P.Id = PA.ProviderId
                            LEFT OUTER JOIN ProviderRegistrationDetail PRD on P.Ukprn = PRD.Ukprn 
		                    WHERE pc.LarsCode={larsCode}
                            AND PRD.StatusId in ({OrganisationStatus.Active}, {OrganisationStatus.ActiveNotTakingOnApprentices})
                            AND PRD.ProviderTypeId={ProviderType.Main}";
        }

        private static FormattableString GetProviderLocationDetailsWithDistanceSql(int ukprn, int larsCode, decimal? lat, decimal? lon)
        {
            return $@"
                    SELECT  P.Id as providerId,
                    LocationType,
                    PL.AddressLine1,
                    PL.AddressLine2,
                    PL.Town,
                    PL.County,
                    PL.Postcode,
	                PCL.HasDayReleaseDeliveryOption,
	                PCL.HasBlockReleaseDeliveryOption,
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
                      LEFT OUTER JOIN ProviderRegistrationDetail PRD on P.Ukprn = PRD.Ukprn 
                      WHERE P.Ukprn={ukprn}
                      AND LarsCode={larsCode}
                      AND PRD.StatusId in ({OrganisationStatus.Active}, {OrganisationStatus.ActiveNotTakingOnApprentices})";

        }

        private static FormattableString GetAllProviderLocationDetailsWithDistanceSql(int larsCode, decimal? lat, decimal? lon)
        {
            return $@"
                    SELECT P.Ukprn,
                    P.Id as providerId,
                    PC.LarsCode,
                    LocationType,
                    PL.AddressLine1,
                    PL.AddressLine2,
                    PL.Town,
                    PL.County,
                    PL.Postcode,
	                PCL.HasDayReleaseDeliveryOption,
	                PCL.HasBlockReleaseDeliveryOption,
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
                LEFT OUTER JOIN ProviderRegistrationDetail PRD on P.Ukprn = PRD.Ukprn 
                WHERE  LarsCode={larsCode}
                AND PRD.StatusId in ({OrganisationStatus.Active}, {OrganisationStatus.ActiveNotTakingOnApprentices})
                AND PRD.ProviderTypeId={ProviderType.Main}";
        }
    }
}
