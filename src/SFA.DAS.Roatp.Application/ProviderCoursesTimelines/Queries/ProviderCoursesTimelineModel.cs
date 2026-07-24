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

    public static implicit operator ProviderCoursesTimelineModel(List<ProviderTimelineExport> providerTimelineExport)
    {
        if (providerTimelineExport is null || providerTimelineExport.Count == 0)
        {
            return null;
        }

        var provider = providerTimelineExport[0];

        return new ProviderCoursesTimelineModel
        {
            Ukprn = provider.Ukprn,
            Status = (ProviderStatusType)provider.StatusId,
            ProviderType = (ProviderType)provider.ProviderTypeId,
            CourseTypes = providerTimelineExport
                .Where(t => t.CourseType.HasValue)
                .GroupBy(t => t.CourseType.Value)
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
