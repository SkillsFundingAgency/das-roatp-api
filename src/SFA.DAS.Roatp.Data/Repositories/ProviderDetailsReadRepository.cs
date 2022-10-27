using System;
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

        public async Task<ProviderDetailsWithDistance> GetProviderDetailsWithDistance(int ukprn, double? lat, double? lon)
        {
            var provider = await _roatpDataContext.ProviderDetailsWithDistance
                .FromSqlInterpolated(GetProvidersDetailsWithDistanceSQL(ukprn, lat, lon)).FirstOrDefaultAsync();
            return provider;
        }



        private FormattableString GetProvidersDetailsWithDistanceSQL(int ukprn, double? lat, double? lon )
        {
            return $@"
                    select pdv.ukprn,
                      pdv.LegalName,
                      pdv.TradingName,
                      pdv.MarketingInfo,
                      pdv.Email,
                      pdv.Phone,
                      pdv.Website,
                      pdv.AddressLine1,
                      pdv.AddressLine2,
                      pdv.AddressLine3,
                      pdv.AddressLine4,
                      pdv.Town,
                      Pdv.Postcode,
                      PDv.Latitude,
                      Pdv.Longitude,
                      CASE  WHEN ({lat} is null) THEN 0
                            WHEN ({lon} is null) THEN 0
                            ELSE
                              geography::Point(isnull(pdv.Latitude,0), isnull(pdv.Longitude,0), 4326)
                                        .STDistance(geography::Point({lat}, {lon}, 4326)) * 0.0006213712 END
			                            as Distance
                      from providerDetailsView pdv
                      where ukprn={ukprn}";
        }
    }
}
