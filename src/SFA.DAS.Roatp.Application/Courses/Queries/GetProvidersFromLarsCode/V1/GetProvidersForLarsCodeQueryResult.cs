using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1
{
    public class GetProvidersForLarsCodeQueryResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int LarsCode { get; set; }
        public string StandardName { get; set; }
        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }

        public List<ProviderData> Providers { get; set; }

        public static implicit operator GetProvidersForLarsCodeQueryResult(GetProvidersForLarsCodeQueryResultV2 v2)
        {
            if (v2 == null) return null;

            return new GetProvidersForLarsCodeQueryResult
            {
                Page = v2.Page,
                PageSize = v2.PageSize,
                TotalPages = v2.TotalPages,
                TotalCount = v2.TotalCount,
                LarsCode = v2.LarsCode,
                StandardName = v2.StandardName,
                QarPeriod = v2.QarPeriod,
                ReviewPeriod = v2.ReviewPeriod,
                Providers = v2.Providers?.Select(p => new ProviderData
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