using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;
using SFA.DAS.Roatp.Domain.Constants;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderRestrictedCourses.Queries.GetProviderRestrictedApprenticeships;

public class RestrictedApprenticeshipModelTests
{
    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_ReturnsExpectedModel(Standard standard)
    {
        // Act
        RestrictedApprenticeshipModel sut = standard;

        // Assert
        sut.LarsCode.Should().Be(standard.LarsCode);
        sut.Title.Should().Be(standard.Title);
        sut.Level.Should().Be(standard.Level);
        sut.LastDateStarts.Should().Be(standard.LastDateStarts);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsStartRestrictedDate_SetsLastDateStartsToNull(Standard standard)
    {
        // Arrange
        standard.LastDateStarts = DateConstants.StartRestrictedDate;

        // Act
        RestrictedApprenticeshipModel sut = standard;

        // Assert
        sut.LastDateStarts.Should().BeNull();
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsNotStartRestrictedDate_SetsLastDateStarts(Standard standard)
    {
        // Arrange
        var lastDateStarts = DateConstants.StartRestrictedDate.AddDays(1);
        standard.LastDateStarts = lastDateStarts;

        // Act
        RestrictedApprenticeshipModel sut = standard;

        // Assert
        sut.LastDateStarts.Should().Be(lastDateStarts);
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_WhenLastDateStartsIsNull_SetsLastDateStartsToNull(Standard standard)
    {
        // Arrange
        standard.LastDateStarts = null;

        // Act
        RestrictedApprenticeshipModel sut = standard;

        // Assert
        sut.LastDateStarts.Should().BeNull();
    }
}
