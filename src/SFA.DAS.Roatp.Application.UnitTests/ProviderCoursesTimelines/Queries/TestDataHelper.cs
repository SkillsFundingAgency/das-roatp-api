using System;
using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries;

public static class TestDataHelper
{
    public static ProviderRegistrationDetail GetProviderRegistrationDetails()
        => new()
        {
            Ukprn = 12345678,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            Provider = new Domain.Entities.Provider
            {
                ProviderCourseTypes = new List<Domain.Entities.ProviderCourseType>
                    {
                        new Domain.Entities.ProviderCourseType
                        {
                            CourseType = CourseType.Apprenticeship,
                        }
                    },
                ProviderCoursesTimelines = new List<Domain.Entities.ProviderCoursesTimeline>
                    {
                        new Domain.Entities.ProviderCoursesTimeline
                        {
                            LarsCode = "LARS123",
                            EffectiveFrom = new DateTime(2023, 1, 1, 0,0,0, DateTimeKind.Unspecified),
                            EffectiveTo = null,
                            Standard = new Domain.Entities.Standard { CourseType = CourseType.Apprenticeship }
                        }
                    }
            }
        };
}
