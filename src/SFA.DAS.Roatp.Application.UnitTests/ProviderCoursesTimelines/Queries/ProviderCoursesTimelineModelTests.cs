using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries;

public class ProviderCoursesTimelineModelTests
{
    ProviderRegistrationDetail _expected = null;
    ProviderCoursesTimelineModel _actual = null;
    ProviderRegistrationDetail _expectedCourseNotAllowed = null;
    ProviderCoursesTimelineModel _actualCourseNotAllowed = null;

    [SetUp]
    public void BeforeEachTest()
    {
        _expected = TestDataHelper.GetProviderRegistrationDetails();
        _actual = _expected;
        _expectedCourseNotAllowed = TestDataHelper.GetProviderRegistrationDetailsCourseIsNotAllowed();
        _actualCourseNotAllowed = _expectedCourseNotAllowed;
    }

    [Test]
    public void Operator_ReturnsNull()
    {
        ProviderRegistrationDetail expected = null;

        ProviderCoursesTimelineModel actual = expected;

        actual.Should().BeNull();
    }

    [Test]
    public void Operator_ConvertsFromProviderRegistrationDetails() => _actual.Should().NotBeNull();

    [Test]
    public void Operator_SetsUkprn() => _actual.Ukprn.Should().Be(_expected.Ukprn);

    [Test]
    public void Operator_SetsProviderStatusType() => _actual.Status.Should().Be(ProviderStatusType.Active);

    [Test]
    public void Operator_SetsProviderType() => _actual.ProviderType.Should().Be(ProviderType.Main);

    [Test]
    public void Operator_SetsCourseTypes() => _actual.CourseTypes.Should().HaveCount(2);

    [Test]
    public void Operator_HasNoCourseForCourseType_SetsEmptyCourse() => _actual.CourseTypes.First(c => c.CourseType == CourseType.ShortCourse).Courses.Should().HaveCount(0);

    [Test]
    public void Operator_HasCourseForCourseType_SetsCourse() => _actual.CourseTypes.First(c => c.CourseType == CourseType.Apprenticeship).Courses.Should().HaveCount(1);

    [Test]
    public void Operator_SetsLarsCodeInCourses()
    {
        var expected = _expected.Provider.ProviderCoursesTimelines[0];
        var actual = _actual.CourseTypes.First(c => c.CourseType == CourseType.Apprenticeship).Courses.First();
        actual.LarsCode.Should().Be(expected.LarsCode);
        actual.EffectiveFrom.Should().Be(expected.EffectiveFrom);
        actual.EffectiveTo.Should().Be(expected.EffectiveTo);
    }

    [Test]
    public void Operator_SetsLastDateStartsWhenAllowedCourseMatchesProviderCourse()
    {
        var expected = _expected.ProviderAllowedCourses[0].LastDateStarts;
        var actual = _actual.CourseTypes.First(c => c.CourseType == CourseType.Apprenticeship).Courses.First(); actual.LastDateStarts.Should().Be(expected);
    }

    [Test]
    public void Operator_SetsLastDateStartsToNullWhenCourseIsNotAllowed()
    {
        var actual = _actualCourseNotAllowed.CourseTypes.First(c => c.CourseType == CourseType.Apprenticeship).Courses.First(); actual.LastDateStarts.Should().BeNull();
    }
}
