using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries.GetAllProviderCoursesTimelines;

public class GetAllProviderCoursesTimelinesQueryResultTests
{
    [Test]
    public void Operator_ConvertsFromProviderRegistrationDetails()
    {
        var providerRegistrationDetails = new List<Domain.Entities.ProviderRegistrationDetail>
        {
            new Domain.Entities.ProviderRegistrationDetail
            {
                Ukprn = 12345678,
                StatusId = (int) ProviderStatusType.Active,
                ProviderTypeId = (int) ProviderType.Main,
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
            }
        };
        GetAllProviderCoursesTimelinesQueryResult result = providerRegistrationDetails;
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Providers.Count());
        var providerModel = result.Providers.First();
        Assert.AreEqual(12345678, providerModel.Ukprn);
        Assert.AreEqual(ProviderStatusType.Active, providerModel.Status);
        Assert.AreEqual(ProviderType.Main, providerModel.ProviderType);
        Assert.AreEqual(1, providerModel.CourseTypes.Count());
        var courseTypeModel = providerModel.CourseTypes.First();
        Assert.AreEqual(CourseType.Apprenticeship, courseTypeModel.CourseType);
        Assert.AreEqual(1, courseTypeModel.Courses.Count());
        var courseTimelineModel = courseTypeModel.Courses.First();
        Assert.AreEqual("LARS123", courseTimelineModel.LarsCode);
        Assert.AreEqual(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), courseTimelineModel.EffectiveFrom);
        Assert.IsNull(courseTimelineModel.EffectiveTo);
    }
}
