using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersForLarsCodeQueryValidatorTests
{
    private Mock<IStandardsReadRepository> _standardsReadRepo;

    [SetUp]
    public void Before_each_test()
    {
        _standardsReadRepo = new Mock<IStandardsReadRepository>();
        _standardsReadRepo.Setup(x => x.GetStandard(It.IsAny<string>())).ReturnsAsync(new Standard());
    }

    [TestCase("", false)]
    [TestCase("1", true)]
    public async Task Validate_LarsCode(string larsCode, bool isValid)
    {
        var request = new GetProvidersFromLarsCodeRequest();
        var validator = new GetProvidersForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetProvidersForLarsCodeQuery(larsCode, request));

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.LarsCode);
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
            result.Errors[0].ErrorMessage.Should().Be(LarsCodeValidator.InvalidMessage);
        }
    }


    [TestCase(180, false)]
    [TestCase(1, true)]
    [TestCase(-180, false)]
    public async Task Validate_LatitudeRange(int latitude, bool isValid)
    {
        var larsCode = "1";
        var request = new GetProvidersFromLarsCodeRequest
        { Latitude = latitude, Longitude = 0, OrderBy = ProviderOrderBy.Distance };
        var validator = new GetProvidersForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetProvidersForLarsCodeQuery(larsCode, request));

        if (isValid)
            result.ShouldNotHaveValidationErrorFor(c => c.Latitude.HasValue);
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.Latitude.HasValue);
            result.Errors[0].ErrorMessage.Should().Be(CoordinatesValidator.LatitudeOutsideAcceptableRange);
        }
    }

    [TestCase(90, 0, true, "")]
    [TestCase(-90, 0, true, "")]
    [TestCase(0, 180, true, "")]
    [TestCase(0, -180, true, "")]
    [TestCase(null, null, true, "")]
    [TestCase(90.0001, 0, false, CoordinatesValidator.LatitudeOutsideAcceptableRange)]
    [TestCase(-90.0001, 0, false, CoordinatesValidator.LatitudeOutsideAcceptableRange)]
    [TestCase(0, 180.0001, false, CoordinatesValidator.LongitudeOutsideAcceptableRange)]
    [TestCase(0, -180.0001, false, CoordinatesValidator.LongitudeOutsideAcceptableRange)]
    [TestCase(56, null, false, CoordinatesValidator.LatitudeAndNotLongitude)]
    [TestCase(null, 0, false, CoordinatesValidator.NotLatitudeAndLongitude)]
    public async Task Validate_LatitudeLongitude(decimal? latitude, decimal? longitude, bool isValid,
        string errorMessage)
    {
        var larsCode = "1";
        var request = new GetProvidersFromLarsCodeRequest
        { Latitude = latitude, Longitude = longitude, OrderBy = ProviderOrderBy.Distance };
        var validator = new GetProvidersForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetProvidersForLarsCodeQuery(larsCode, request));
        result.IsValid.Should().Be(isValid);
        if (!result.IsValid) result.Errors[0].ErrorMessage.Should().Be(errorMessage);
    }


    [TestCase(null, false)]
    [TestCase(ProviderOrderBy.Distance, true)]
    [TestCase(ProviderOrderBy.AchievementRate, true)]
    [TestCase(ProviderOrderBy.ApprenticeProviderRating, true)]
    [TestCase(ProviderOrderBy.EmployerProviderRating, true)]
    public async Task Validate_OrderBy(ProviderOrderBy? orderBy, bool isValid)
    {
        var larsCode = "1";
        var request = new GetProvidersFromLarsCodeRequest { Latitude = 80, Longitude = 0, OrderBy = orderBy };
        var validator = new GetProvidersForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetProvidersForLarsCodeQuery(larsCode, request));
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.OrderBy);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.OrderBy);
            result.Errors[0].ErrorMessage.Should().Be(GetProvidersForLarsCodeQueryValidator.OrderByRequiredErrorMessage);
        }
    }

    [TestCase(null, true)]
    [TestCase(10, true)]
    [TestCase(0, false)]
    [TestCase(-1, false)]
    public async Task Validate_Distance(int? distance, bool isValid)
    {
        var larsCode = "1";
        var request = new GetProvidersFromLarsCodeRequest { Latitude = 80, Longitude = 0, OrderBy = ProviderOrderBy.Distance, Distance = distance };
        var validator = new GetProvidersForLarsCodeQueryValidator(_standardsReadRepo.Object);

        var result = await validator.TestValidateAsync(new GetProvidersForLarsCodeQuery(larsCode, request));
        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.Distance);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.Distance);
            result.Errors[0].ErrorMessage.Should().Be(GetProvidersForLarsCodeQueryValidator.DistanceErrorMessage);
        }
    }
}