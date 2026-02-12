using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries;

public class ProviderCoursesTimelineModelTests
{
    [Test]
    public void Operator_ConvertsFromProviderRegistrationDetails()
    {
        var providerRegistrationDetails = TestDataHelper.GetProviderRegistrationDetails();
        ProviderCoursesTimelineModel providerModel = providerRegistrationDetails;
        Assert.IsNotNull(providerModel);
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
