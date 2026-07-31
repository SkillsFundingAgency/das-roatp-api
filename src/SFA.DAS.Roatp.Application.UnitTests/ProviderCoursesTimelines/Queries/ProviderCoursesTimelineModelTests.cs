using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCoursesTimelines.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCoursesTimelines.Queries;

public class ProviderCoursesTimelineModelTests
{
    private List<ProviderTimelineExport> _expected = null!;
    private ProviderCoursesTimelineModel _actual = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _expected = new List<ProviderTimelineExport>
    {
        new()
        {
            Ukprn = 100001,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            CourseType = CourseType.Apprenticeship,
            LarsCode = "100",
            EffectiveFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EffectiveTo = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            LastDateStarts = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Ukprn = 100001,
            StatusId = (int)ProviderStatusType.Active,
            ProviderTypeId = (int)ProviderType.Main,
            CourseType = CourseType.ShortCourse,
            LarsCode = null,
            EffectiveFrom = null,
            EffectiveTo = null,
            LastDateStarts = null
        }
    };

        _actual = _expected;
    }

    [Test]
    public void Operator_ReturnsNull_ForNullList()
    {
        List<ProviderTimelineExport> expected = null!;

        ProviderCoursesTimelineModel actual = expected;

        actual.Should().BeNull();
    }

    [Test]
    public void Operator_ReturnsNull_ForEmptyList()
    {
        List<ProviderTimelineExport> expected = [];

        ProviderCoursesTimelineModel actual = expected;

        actual.Should().BeNull();
    }

    [Test]
    public void Operator_ConvertsFromProviderCoursesTimelineExportList()
    {
        _actual.Should().NotBeNull();
    }

    [Test]
    public void Operator_SetsUkprn()
    {
        _actual.Ukprn.Should().Be(100001);
    }

    [Test]
    public void Operator_SetsProviderStatusType()
    {
        _actual.Status.Should().Be(ProviderStatusType.Active);
    }

    [Test]
    public void Operator_SetsProviderType()
    {
        _actual.ProviderType.Should().Be(ProviderType.Main);
    }

    [Test]
    public void Operator_SetsCourseTypes()
    {
        _actual.CourseTypes.Should().HaveCount(2);
    }

    [Test]
    public void Operator_HasNoCourseForCourseType_SetsEmptyCourse()
    {
        _actual.CourseTypes
            .First(c => c.CourseType == CourseType.ShortCourse)
            .Courses
            .Should()
            .BeEmpty();
    }

    [Test]
    public void Operator_HasCourseForCourseType_SetsCourse()
    {
        _actual.CourseTypes
            .First(c => c.CourseType == CourseType.Apprenticeship)
            .Courses
            .Should()
            .HaveCount(1);
    }

    [Test]
    public void Operator_SetsLarsCodeInCourses()
    {
        var expected = _expected.First(x => x.CourseType == CourseType.Apprenticeship);

        var actual = _actual.CourseTypes
            .First(c => c.CourseType == CourseType.Apprenticeship)
            .Courses
            .First();

        actual.LarsCode.Should().Be(expected.LarsCode);
        actual.EffectiveFrom.Should().Be(expected.EffectiveFrom);
        actual.EffectiveTo.Should().Be(expected.EffectiveTo);
        actual.LastDateStarts.Should().Be(expected.LastDateStarts);
    }
}
