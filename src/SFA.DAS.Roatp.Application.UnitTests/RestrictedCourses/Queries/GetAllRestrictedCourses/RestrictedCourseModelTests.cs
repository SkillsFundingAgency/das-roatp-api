using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.RestrictedCourses.Queries.GetAllRestrictedCourses;

public class RestrictedCourseModelTests
{
    [Test]
    public void ImplicitConversion_ReturnsExpectedModel()
    {
        // Arrange
        var source = new Standard()
        {
            Title = "Test Course",
            Level = 1,
            LarsCode = "123456ABC",
            LearningType = LearningType.FoundationApprenticeship
        };

        // Act
        RestrictedCourseModel sut = source;

        // Assert
        sut.LarsCode.Should().Be(source.LarsCode);
        sut.Title.Should().Be(source.Title);
        sut.Level.Should().Be(source.Level);
        sut.LearningType.Should().Be(source.LearningType);
    }
}
