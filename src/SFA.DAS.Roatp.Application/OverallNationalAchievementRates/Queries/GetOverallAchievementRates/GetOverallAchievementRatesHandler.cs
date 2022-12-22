using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates
{
    public class GetOverallAchievementRatesHandler : IRequestHandler<GetOverallAchievementRatesQuery, GetOverallAchievementRatesQueryResult>
    {
        private readonly INationalAchievementRatesOverallReadRepository _nationalAchievementRatesOverallReadRepository;

        public GetOverallAchievementRatesHandler(INationalAchievementRatesOverallReadRepository nationalAchievementRatesImportReadRepository)
        {
            _nationalAchievementRatesOverallReadRepository = nationalAchievementRatesImportReadRepository;
        }
        public async Task<GetOverallAchievementRatesQueryResult> Handle(GetOverallAchievementRatesQuery request, CancellationToken cancellationToken)
        {
            var result = await _nationalAchievementRatesOverallReadRepository.GetBySectorSubjectArea(request.SectorSubjectArea);

            return new GetOverallAchievementRatesQueryResult
            {
                OverallAchievementRates = result?.Select(x => (NationalAchievementRateOverallModel)x).ToList()
            };
        }
    }
}