using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class ProviderAllowedCourseModelTests
{
    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromProviderAllowedCourse_ReturnsExpectedModel(
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LarsCode.Should().Be(providerAllowedCourse.LarsCode);
        sut.Title.Should().Be(providerAllowedCourse.Standard.Title);
        sut.Level.Should().Be(providerAllowedCourse.Standard.Level);
    }

    [Test]
    public void ImplicitConversionFromProviderAllowedCourse_WhenLastDateStartsIsStartRestrictedDate_SetsLastDateStartsToNullAndIsClosedToNewStartsToTrue()
    {
        // Arrange
        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = DateConstants.StartRestrictedDate,
            Standard = new Standard()
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().BeNull();
        sut.IsClosedToNewStarts.Should().BeTrue();
    }

    [Test]
    public void ImplicitConversionFromProviderAllowedCourse_WhenLastDateStartsIsInPast_SetsLastDateStartsAndIsClosedToNewStarts()
    {
        // Arrange
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(-1);

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = lastDateStarts,
            Standard = new Standard()
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().Be(lastDateStarts);
        sut.IsClosedToNewStarts.Should().BeTrue();
    }

    [Test]
    public void ImplicitConversionFromProviderAllowedCourse_WhenLastDateStartsIsToday_SetsLastDateStartsAndIsClosedToNewStartsToFalse()
    {
        // Arrange
        var lastDateStarts = DateTime.UtcNow.Date;

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = lastDateStarts,
            Standard = new Standard()
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().Be(lastDateStarts);
        sut.IsClosedToNewStarts.Should().BeFalse();
    }

    [Test]
    public void ImplicitConversionFromProviderAllowedCourse_WhenLastDateStartsIsInFuture_SetsLastDateStartsAndIsClosedToNewStartsToFalse()
    {
        // Arrange
        var lastDateStarts = DateTime.UtcNow.Date.AddDays(1);

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = lastDateStarts,
            Standard = new Standard()
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().Be(lastDateStarts);
        sut.IsClosedToNewStarts.Should().BeFalse();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandard_ReturnsExpectedModel(
        Standard standard)
    {
        // Act
        ProviderAllowedCourseModel sut = standard;

        // Assert
        sut.LarsCode.Should().Be(standard.LarsCode);
        sut.Title.Should().Be(standard.Title);
        sut.Level.Should().Be(standard.Level);
        sut.LastDateStarts.Should().BeNull();
        sut.IsClosedToNewStarts.Should().BeFalse();
    }
}