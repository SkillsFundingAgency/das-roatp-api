using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RegionReadRepository : IRegionReadRepository
    {
        private readonly RoatpDataContext _roatpDataContext;

        public RegionReadRepository(RoatpDataContext roatpDataContext)
        {
            _roatpDataContext = roatpDataContext;
        }

        public async Task<List<Region>> GetAllRegions()
        {
            return await _roatpDataContext
                .Regions
                .AsNoTracking()
                .ToListAsync();
        }
    }
}