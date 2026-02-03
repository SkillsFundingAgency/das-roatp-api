using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;

namespace SFA.DAS.Roatp.Api.Models.V1
{
    public class GetProvidersForLarsCodeResultModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int LarsCode { get; set; }
        public string StandardName { get; set; }
        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }

        public List<ProviderData> Providers { get; set; } = new List<ProviderData>();

        public static implicit operator GetProvidersForLarsCodeResultModel(GetProvidersForLarsCodeQueryResult source)
        {
            if (source == null) return null;
            return new GetProvidersForLarsCodeResultModel
            {
                Page = source.Page,
                PageSize = source.PageSize,
                TotalPages = source.TotalPages,
                TotalCount = source.TotalCount,
                LarsCode = int.Parse(source.LarsCode),
                StandardName = source.StandardName,
                QarPeriod = source.QarPeriod,
                ReviewPeriod = source.ReviewPeriod,
                Providers = source.Providers.Select(p => new ProviderData
                {
                    Ordering = p.Ordering,
                    Ukprn = p.Ukprn,
                    ProviderName = p.ProviderName,
                    ShortlistId = p.ShortlistId,
                    Locations = p.Locations,
                    Leavers = p.Leavers,
                    AchievementRate = p.AchievementRate,
                    EmployerReviews = p.EmployerReviews,
                    EmployerStars = p.EmployerStars,
                    EmployerRating = p.EmployerRating,
                    ApprenticeReviews = p.ApprenticeReviews,
                    ApprenticeStars = p.ApprenticeStars,
                    ApprenticeRating = p.ApprenticeRating
                }).ToList()
            };
        }
    }


}