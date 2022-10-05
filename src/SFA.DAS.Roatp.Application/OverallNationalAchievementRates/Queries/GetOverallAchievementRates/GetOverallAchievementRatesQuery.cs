using MediatR;

namespace SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates
{
    public class GetOverallAchievementRatesQuery : IRequest<GetOverallAchievementRatesQueryResult>
    {
        public string SectorSubjectArea { get; set; }
    }
}