using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryResult
{
    public IEnumerable<ProviderCoursesTimelineModel> Providers { get; set; } = [];

    public static implicit operator GetAllProviderCoursesTimelinesQueryResult(List<ProviderRegistrationDetail> providers)
    {
        GetAllProviderCoursesTimelinesQueryResult result = new()
        {
            Providers = providers.Select(p => new ProviderCoursesTimelineModel
            {
                Ukprn = p.Ukprn,
                Status = (ProviderStatusType)p.StatusId,
                ProviderType = (ProviderType)p.ProviderTypeId,
                CourseTypes = p.Provider?.ProviderCourseTypes
                    .Select(ct => new CourseTypeModel(
                        ct.CourseType,
                        p.Provider.ProviderCoursesTimelines
                            .Where(t => t.Standard.CourseType == ct.CourseType)
                            .Select(t => new CoursesTimelineModel(t.LarsCode, t.EffectiveFrom, t.EffectiveTo))))
                    ?? []
            })
        };
        return result;
    }
}

public class ProviderCoursesTimelineModel
{
    public int Ukprn { get; set; }
    public ProviderStatusType Status { get; set; }
    public ProviderType ProviderType { get; set; }
    public IEnumerable<CourseTypeModel> CourseTypes { get; set; } = [];
}

public record CourseTypeModel(CourseType CourseType, IEnumerable<CoursesTimelineModel> Courses);

public record CoursesTimelineModel(string LarsCode, DateTime EffectiveFrom, DateTime? EffectiveTo);
