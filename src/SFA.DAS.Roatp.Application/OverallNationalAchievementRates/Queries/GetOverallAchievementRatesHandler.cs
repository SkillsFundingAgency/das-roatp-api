using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CourseDelivery.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries
{
    public class GetOverallAchievementRatesHandler : IRequestHandler<GetOverallAchievementRatesQuery, GetOverallAchievementRatesResponse>
    {
        private readonly IOverallNationalAchievementRateService _service;

        public GetOverallAchievementRatesHandler (IOverallNationalAchievementRateService service)
        {
            _service = service;
        }
        public async Task<GetOverallAchievementRatesResponse> Handle(GetOverallAchievementRatesQuery request, CancellationToken cancellationToken)
        {
            var result = await _service.GetItemsBySectorSubjectArea(request.SectorSubjectArea);
            
            return new GetOverallAchievementRatesResponse
            {
                OverallAchievementRates = result
            }; 
        }
    }
}