using MediatR;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries
{
    public class GetOverallAchievementRatesHandler : IRequestHandler<GetOverallAchievementRatesQuery, GetOverallAchievementRatesResponse>
    {
        private readonly INationalAchievementRatesOverallReadRepository _nationalAchievementRatesOverallReadRepository;

        public GetOverallAchievementRatesHandler(INationalAchievementRatesOverallReadRepository nationalAchievementRatesImportReadRepository)
        {
            _nationalAchievementRatesOverallReadRepository = nationalAchievementRatesImportReadRepository;
        }
        public async Task<GetOverallAchievementRatesResponse> Handle(GetOverallAchievementRatesQuery request, CancellationToken cancellationToken)
        {
            var result = await _nationalAchievementRatesOverallReadRepository.GetBySectorSubjectArea(request.SectorSubjectArea);
            
            return new GetOverallAchievementRatesResponse
            {
                OverallAchievementRates = result
            }; 
        }
    }
}