﻿using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;

public class NationalAchievementRateModel
{
    public Age Age { get; set; }

    public ApprenticeshipLevel Level { get; set; }

    public decimal? OverallAchievementRate { get; set; }

    public int? OverallCohort { get; set; }

    public static implicit operator NationalAchievementRateModel(NationalAchievementRate source) =>
        new()
        {
            Age = source.Age,
            Level = source.ApprenticeshipLevel,
            OverallAchievementRate = source.OverallAchievementRate,
            OverallCohort = source.OverallCohort
        };
}