using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourses;

public sealed class GetCoursesQueryValidatorTests
{
    [Test]
    public async Task Validate_WhenLongitudeAndLatitudeHaveValues_DistanceMustNotBeNull()
    {
        var query = new GetCoursesQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = new int[] { 1 },
            Distance = null
        };

        var sut = new GetCoursesQueryValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(c => c.Distance).WithErrorMessage(GetCoursesQueryValidator.DistanceValidationMessage);
    }

    [Test]
    public async Task Validate_WhenLarsCodesIsEmpty_QueryIsInvalid()
    {
        var query = new GetCoursesQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = new int[] {  },
            Distance = null
        };

        var sut = new GetCoursesQueryValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(c => c.LarsCodes).WithErrorMessage(GetCoursesQueryValidator.LarsCodesValidationMessage);
    }

    [Test]
    public async Task Validate_WhenLarsCodesIsPopulated_QueryIsValid()
    {
        var query = new GetCoursesQuery()
        {
            Longitude = 90,
            Latitude = -90,
            LarsCodes = new int[] { 1 },
            Distance = null
        };

        var sut = new GetCoursesQueryValidator();

        var result = await sut.TestValidateAsync(query);

        result.ShouldNotHaveValidationErrorFor(c => c.LarsCodes);
    }
}
