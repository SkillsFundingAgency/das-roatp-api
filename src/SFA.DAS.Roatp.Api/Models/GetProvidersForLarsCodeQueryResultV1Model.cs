using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1
{
    public class GetProvidersForLarsCodeQueryResultV1Model
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int LarsCode { get; set; }
        public string StandardName { get; set; }
        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }

        public List<ProviderDataV1> Providers { get; set; }

        public static implicit operator GetProvidersForLarsCodeQueryResultV1Model(GetProvidersForLarsCodeQueryResult v2)
        {
            if (v2 == null) return null;
            return new GetProvidersForLarsCodeQueryResultV1Model
            {
                Page = v2.Page,
                PageSize = v2.PageSize,
                TotalPages = v2.TotalPages,
                TotalCount = v2.TotalCount,
                LarsCode = int.TryParse(v2.LarsCode, out int larsCodeValue) ? larsCodeValue : 0,
                StandardName = v2.StandardName,
                QarPeriod = v2.QarPeriod,
                ReviewPeriod = v2.ReviewPeriod,
                Providers = v2.Providers?.Select(p => new ProviderDataV1
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

    public class ProviderDataV1
    {
        public long Ordering { get; set; }
        public int Ukprn { get; set; }
        public string ProviderName { get; set; }
        public Guid? ShortlistId { get; set; }
        public List<ProviderLocationModel> Locations { get; set; }
        public string Leavers { get; set; }
        public string AchievementRate { get; set; }
        public string EmployerReviews { get; set; }
        public string EmployerStars { get; set; }
        public ProviderRating EmployerRating { get; set; }
        public string ApprenticeReviews { get; set; }
        public string ApprenticeStars { get; set; }
        public ProviderRating ApprenticeRating { get; set; }
    }
}