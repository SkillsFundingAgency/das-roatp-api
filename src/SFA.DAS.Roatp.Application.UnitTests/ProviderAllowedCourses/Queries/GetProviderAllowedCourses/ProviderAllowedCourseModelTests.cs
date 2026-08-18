using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class ProviderAllowedCourseModelTests
{
    private static readonly DateTime StartRestrictedDate = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_ReturnsExpectedModel(
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LarsCode.Should().Be(providerAllowedCourse.LarsCode);
        sut.Title.Should().Be(providerAllowedCourse.Standard.Title);
        sut.Level.Should().Be(providerAllowedCourse.Standard.Level);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ImplicitConversion_SetsLastDateStartsAndIsStartRestricted(
        bool isStartRestricted)
    {
        // Arrange
        var lastDateStarts = isStartRestricted
            ? StartRestrictedDate
            : DateTime.UtcNow;

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = lastDateStarts,
            Standard = new Standard()
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().Be(isStartRestricted ? null : lastDateStarts);
        sut.IsStartRestricted.Should().Be(isStartRestricted);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ImplicitConversion_SetsIsActive(bool providerCourseExists)
    {
        // Arrange
        var providerAllowedCourse = new ProviderAllowedCourse
        {
            Standard = new Standard(),
            ProviderCourse = providerCourseExists
                ? new Domain.Entities.ProviderCourse()
                : null
        };

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.IsActive.Should().Be(providerCourseExists);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_ReturnsExpectedModel(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse,
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerCourse.ProviderAllowedCourse = providerAllowedCourse;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.LarsCode.Should().Be(standard.LarsCode);
        sut.Title.Should().Be(standard.Title);
        sut.Level.Should().Be(standard.Level);
        sut.IsActive.Should().BeTrue();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ImplicitConversionFromStandardAndProviderCourse_SetsLastDateStartsAndIsStartRestricted(
        bool isStartRestricted)
    {
        // Arrange
        var lastDateStarts = isStartRestricted
            ? StartRestrictedDate
            : DateTime.UtcNow;

        var standard = new Standard();

        var providerAllowedCourse = new ProviderAllowedCourse
        {
            LastDateStarts = lastDateStarts
        };

        var providerCourse = new Domain.Entities.ProviderCourse
        {
            ProviderAllowedCourse = providerAllowedCourse
        };

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.LastDateStarts.Should().Be(isStartRestricted ? null : lastDateStarts);
        sut.IsStartRestricted.Should().Be(isStartRestricted);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenProviderAllowedCourseDoesNotExist_ReturnsExpectedModel(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse)
    {
        // Arrange
        providerCourse.ProviderAllowedCourse = null;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.LastDateStarts.Should().BeNull();
        sut.IsStartRestricted.Should().BeFalse();
        sut.IsActive.Should().BeTrue();
    }
}