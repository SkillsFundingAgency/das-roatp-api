using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;

public class ProviderCoursesTimelineModel
{
    public int Ukprn { get; set; }
    public ProviderStatusType Status { get; set; }
    public ProviderType ProviderType { get; set; }
    public IEnumerable<CourseTypeModel> CourseTypes { get; set; } = [];

    public static implicit operator ProviderCoursesTimelineModel(List<ProviderCoursesTimelineExport> providerCoursesTimeline)
    {
        if (providerCoursesTimeline is null || providerCoursesTimeline.Count == 0)
        {
            return null;
        }

        var providerCourse = providerCoursesTimeline.FirstOrDefault();

        return new ProviderCoursesTimelineModel
        {
            Ukprn = providerCourse.Ukprn,
            Status = (ProviderStatusType)providerCourse.StatusId,
            ProviderType = (ProviderType)providerCourse.ProviderTypeId,
            CourseTypes = providerCoursesTimeline
                .Where(t => t.CourseType.HasValue)
                .GroupBy(t => t.CourseType!.Value)
                .Select(group => new CourseTypeModel(
                    group.Key,
                    group
                        .Where(t => t.LarsCode != null)
                        .Select(t => new CoursesTimelineModel(
                            t.LarsCode,
                            t.EffectiveFrom,
                            t.EffectiveTo,
                            t.LastDateStarts))
                        .ToList()))
        };
    }
}

public record CourseTypeModel(CourseType CourseType, IEnumerable<CoursesTimelineModel> Courses);

public record CoursesTimelineModel(string LarsCode, DateTime? EffectiveFrom, DateTime? EffectiveTo, DateTime? LastDateStarts);
