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
    public void ImplicitConversion_ReturnsExpectedModel(ProviderAllowedCourse providerAllowedCourse)
    {
        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LarsCode.Should().Be(providerAllowedCourse.LarsCode);
        sut.Title.Should().Be(providerAllowedCourse.Standard.Title);
        sut.Level.Should().Be(providerAllowedCourse.Standard.Level);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsNotStartRestrictedDate_ReturnsLastDateStarts(ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = DateTime.UtcNow;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().Be(providerAllowedCourse.LastDateStarts);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsStartRestrictedDate_ReturnsNullLastDateStarts(ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = StartRestrictedDate;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.LastDateStarts.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsStartRestrictedDate_SetsIsStartRestrictedToTrue(ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = StartRestrictedDate;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.IsStartRestricted.Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsNotStartRestrictedDate_SetsIsStartRestrictedToFalse(ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = DateTime.UtcNow;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.IsStartRestricted.Should().BeFalse();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenProviderCourseExists_SetsIsActiveToTrue(ProviderAllowedCourse providerAllowedCourse,
        Domain.Entities.ProviderCourse providerCourse)
    {
        // Arrange
        providerAllowedCourse.ProviderCourse = providerCourse;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.IsActive.Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenProviderCourseDoesNotExist_SetsIsActiveToFalse(ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.ProviderCourse = null;

        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;

        // Assert
        sut.IsActive.Should().BeFalse();
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
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenLastDateStartsIsNotStartRestrictedDate_ReturnsLastDateStarts(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse,
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = DateTime.UtcNow;
        providerCourse.ProviderAllowedCourse = providerAllowedCourse;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.LastDateStarts.Should().Be(providerAllowedCourse.LastDateStarts);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenLastDateStartsIsStartRestrictedDate_ReturnsNullLastDateStarts(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse,
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = StartRestrictedDate;
        providerCourse.ProviderAllowedCourse = providerAllowedCourse;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.LastDateStarts.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenLastDateStartsIsStartRestrictedDate_SetsIsStartRestrictedToTrue(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse,
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = StartRestrictedDate;
        providerCourse.ProviderAllowedCourse = providerAllowedCourse;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.IsStartRestricted.Should().BeTrue();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenLastDateStartsIsNotStartRestrictedDate_SetsIsStartRestrictedToFalse(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse,
        ProviderAllowedCourse providerAllowedCourse)
    {
        // Arrange
        providerAllowedCourse.LastDateStarts = DateTime.UtcNow;
        providerCourse.ProviderAllowedCourse = providerAllowedCourse;

        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.IsStartRestricted.Should().BeFalse();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_WhenProviderAllowedCourseDoesNotExist_ReturnsNullLastDateStartsAndIsStartRestrictedFalse(
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
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversionFromStandardAndProviderCourse_SetsIsActiveToTrue(
        Standard standard,
        Domain.Entities.ProviderCourse providerCourse)
    {
        // Act
        ProviderAllowedCourseModel sut = (standard, providerCourse);

        // Assert
        sut.IsActive.Should().BeTrue();
    }
}
