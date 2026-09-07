using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.CreateProviderCourseType;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Commands.CreateProviderCourseType;

public class CreateProviderCourseTypeCommandTests
{
    [Test]
    public void WhenConstructingCommand_ThenMapsRequestProperties()
    {
        // Arrange
        int ukprn = 12345678;

        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"],
            UserId = "TestUserId",
            UserDisplayName = "Test User"
        };

        // Act
        var command = new CreateProviderCourseTypeCommand(ukprn, request);

        // Assert
        command.Ukprn.Should().Be(ukprn);
        command.UserId.Should().Be(request.UserId);
        command.UserDisplayName.Should().Be(request.UserDisplayName);
    }

    [Test]
    public void WhenApprenticeshipCourseTypeIsProvided_ThenConvertsToApprenticeshipCourseType()
    {
        // Arrange
        var request = new AddCourseTypesModel
        {
            CourseTypes = ["Apprenticeship"]
        };

        // Act
        var command = new CreateProviderCourseTypeCommand(12345678, request);

        // Assert
        command.CourseTypes.Should().Contain(CourseType.Apprenticeship);
    }

    [Test]
    public void WhenShortCourseTypeIsProvided_ThenConvertsToShortCourseCourseType()
    {
        // Arrange
        var request = new AddCourseTypesModel
        {
            CourseTypes = ["ShortCourse"]
        };

        // Act
        var command = new CreateProviderCourseTypeCommand(12345678, request);

        // Assert
        command.CourseTypes.Should().Contain(CourseType.ShortCourse);
    }

    [Test]
    public void WhenMultipleCourseTypesAreProvided_ThenConvertsAllCourseTypes()
    {
        // Arrange
        var request = new AddCourseTypesModel
        {
            CourseTypes =
            [
                "Apprenticeship",
                "ShortCourse"
            ]
        };

        // Act
        var command = new CreateProviderCourseTypeCommand(12345678, request);

        // Assert
        command.CourseTypes.Should().Equal(
            CourseType.Apprenticeship,
            CourseType.ShortCourse);
    }

    [Test]
    public void WhenCourseTypeHasDifferentCasing_ThenConvertsCourseTypeIgnoringCase()
    {
        // Arrange
        var request = new AddCourseTypesModel
        {
            CourseTypes = ["apprenticeship"]
        };

        // Act
        var command = new CreateProviderCourseTypeCommand(12345678, request);

        // Assert
        command.CourseTypes.Should().Contain(CourseType.Apprenticeship);
    }
}
