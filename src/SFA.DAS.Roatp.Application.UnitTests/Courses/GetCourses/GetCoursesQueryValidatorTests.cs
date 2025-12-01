using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourses;

public sealed class GetCoursesQueryValidatorTests
{
    [Test]
    public async Task Validate_WhenLongitudeAndLatitudeHaveValues_DistanceMustNotBeNull()
    {
        var query = new GetCourseTrainingProvidersCountQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = new int[] { 1 },
            Distance = null
        };

        var sut = new GetCourseTrainingProvidersCountValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(c => c.Distance).WithErrorMessage(GetCourseTrainingProvidersCountValidator.DistanceValidationMessage);
    }

    [Test]
    public async Task Validate_WhenLongitudeAndLatitudeDoNotHaveValues_DistanceCanBeNull()
    {
        var query = new GetCourseTrainingProvidersCountQuery()
        {
            Longitude = null,
            Latitude = null,
            LarsCodes = new int[] { 1 },
            Distance = null
        };

        var sut = new GetCourseTrainingProvidersCountValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(c => c.Distance);
    }

    [Test]
    public async Task Validate_WhenLarsCodesIsEmpty_QueryIsInvalid()
    {
        var query = new GetCourseTrainingProvidersCountQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = Array.Empty<int>(),
            Distance = null
        };

        var sut = new GetCourseTrainingProvidersCountValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(c => c.LarsCodes).WithErrorMessage(GetCourseTrainingProvidersCountValidator.LarsCodesValidationMessage);
    }

    [Test]
    public async Task Validate_WhenLarsCodesIsPopulated_QueryIsValid()
    {
        var query = new GetCourseTrainingProvidersCountQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = new int[] { 1 },
            Distance = null
        };

        var sut = new GetCourseTrainingProvidersCountValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(c => c.LarsCodes);
    }
}
