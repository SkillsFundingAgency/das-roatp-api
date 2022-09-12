using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.Regions.Queries
{
    public class RegionsQueryHandler : IRequestHandler<RegionsQuery, RegionsQueryResult>
    {
        private readonly IRegionsReadRepository _regionReadRepository;

        public RegionsQueryHandler(IRegionsReadRepository regionReadRepository)
        {
            _regionReadRepository = regionReadRepository;
        }

        public async Task<RegionsQueryResult> Handle(RegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _regionReadRepository.GetAllRegions();
            var result = new RegionsQueryResult
            {
                Regions = regions.Select(x => (RegionModel)x).ToList()
            };
            return result;
        }
    }
}
