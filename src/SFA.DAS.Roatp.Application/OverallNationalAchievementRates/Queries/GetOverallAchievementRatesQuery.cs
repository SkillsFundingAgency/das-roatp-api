using MediatR;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries
{
    public class GetOverallAchievementRatesQuery : IRequest<GetOverallAchievementRatesResponse>
    {
        public string SectorSubjectArea { get ; set ; }
    }
}