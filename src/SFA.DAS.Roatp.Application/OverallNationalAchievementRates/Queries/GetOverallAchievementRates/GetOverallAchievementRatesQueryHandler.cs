using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates
{
    public class GetOverallAchievementRatesQueryHandler : IRequestHandler<GetOverallAchievementRatesQuery, GetOverallAchievementRatesQueryResult>
    {
        private readonly INationalAchievementRatesOverallReadRepository _nationalAchievementRatesOverallReadRepository;

        public GetOverallAchievementRatesQueryHandler(INationalAchievementRatesOverallReadRepository nationalAchievementRatesImportReadRepository)
        {
            _nationalAchievementRatesOverallReadRepository = nationalAchievementRatesImportReadRepository;
        }
        public async Task<GetOverallAchievementRatesQueryResult> Handle(GetOverallAchievementRatesQuery request, CancellationToken cancellationToken)
        {
            var result = await _nationalAchievementRatesOverallReadRepository.GetBySectorSubjectArea(request.SectorSubjectAreaTier1Code);

            return new GetOverallAchievementRatesQueryResult
            {
                OverallAchievementRates = result?.Select(x => (NationalAchievementRateOverallModel)x).ToList()
            };
        }
    }
}