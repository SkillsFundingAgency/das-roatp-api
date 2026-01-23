using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
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

    private static readonly Func<IStandardsReadRepository, IValidator<object>>[] ValidatorFactories = new Func<IStandardsReadRepository, IValidator<object>>[]
    {
        repo => new WrapperValidator<GetProvidersForLarsCodeQuery>(new SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1.GetProvidersForLarsCodeQueryValidator(repo)),
        repo => new WrapperValidator<GetProvidersForLarsCodeQueryV2>(new SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2.GetProvidersForLarsCodeQueryValidator(repo)),
    };

    private static readonly Func<object>[] RequestFactories = new Func<object>[]
    {
        () => new GetProvidersFromLarsCodeRequest(),
        () => new GetProvidersFromLarsCodeRequestV2(),
    };

    private static readonly Func<string, object, object>[] QueryFactories = new Func<string, object, object>[]
    {
        (lars, req) => new GetProvidersForLarsCodeQuery(lars, (GetProvidersFromLarsCodeRequest)req),
        (lars, req) => new GetProvidersForLarsCodeQueryV2(lars, (GetProvidersFromLarsCodeRequestV2)req),
    };

    [SetUp]
    public void Before_each_test()
    {
        _standardsReadRepo = new Mock<IStandardsReadRepository>();
        _standardsReadRepo.Setup(x => x.GetStandard(It.IsAny<string>())).ReturnsAsync(new Standard());
    }

    [TestCase("", false)]
    [TestCase("1", true)]
    public async Task Validate_LarsCode_Works_For_V1_And_V2(string larsCode, bool isValid)
    {
        for (var i = 0; i < ValidatorFactories.Length; i++)
        {
            var request = RequestFactories[i]();
            var validator = ValidatorFactories[i](_standardsReadRepo.Object);
            var query = QueryFactories[i](larsCode, request);

            var result = await validator.TestValidateAsync(query);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor("LarsCode");
            else
            {
                result.ShouldHaveValidationErrorFor("LarsCode");
                result.Errors[0].ErrorMessage.Should().Be(LarsCodeValidator.InvalidMessage);
            }
        }
    }

    [TestCase(180, false)]
    [TestCase(1, true)]
    [TestCase(-180, false)]
    public async Task Validate_LatitudeRange_Works_For_V1_And_V2(int latitude, bool isValid)
    {
        for (var i = 0; i < ValidatorFactories.Length; i++)
        {
            var request = RequestFactories[i]();
            SetProperty(request, "Latitude", latitude);
            SetProperty(request, "Longitude", 0m);
            SetProperty(request, "OrderBy", ProviderOrderBy.Distance);

            var validator = ValidatorFactories[i](_standardsReadRepo.Object);
            var query = QueryFactories[i]("1", request);

            var result = await validator.TestValidateAsync(query);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor("Latitude.HasValue");
            else
            {
                result.ShouldHaveValidationErrorFor("Latitude.HasValue");
                result.Errors[0].ErrorMessage.Should().Be(CoordinatesValidator.LatitudeOutsideAcceptableRange);
            }
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
    public async Task Validate_LatitudeLongitude_Works_For_V1_And_V2(decimal? latitude, decimal? longitude, bool isValid, string errorMessage)
    {
        for (var i = 0; i < ValidatorFactories.Length; i++)
        {
            var request = RequestFactories[i]();
            SetProperty(request, "Latitude", latitude);
            SetProperty(request, "Longitude", longitude);
            SetProperty(request, "OrderBy", ProviderOrderBy.Distance);

            var validator = ValidatorFactories[i](_standardsReadRepo.Object);
            var query = QueryFactories[i]("1", request);

            var result = await validator.TestValidateAsync(query);
            result.IsValid.Should().Be(isValid);
            if (!result.IsValid) result.Errors[0].ErrorMessage.Should().Be(errorMessage);
        }
    }

    [TestCase(null, false)]
    [TestCase(ProviderOrderBy.Distance, true)]
    [TestCase(ProviderOrderBy.AchievementRate, true)]
    [TestCase(ProviderOrderBy.ApprenticeProviderRating, true)]
    [TestCase(ProviderOrderBy.EmployerProviderRating, true)]
    public async Task Validate_OrderBy_Works_For_V1_And_V2(ProviderOrderBy? orderBy, bool isValid)
    {
        for (var i = 0; i < ValidatorFactories.Length; i++)
        {
            var request = RequestFactories[i]();
            SetProperty(request, "Latitude", 80m);
            SetProperty(request, "Longitude", 0m);
            SetProperty(request, "OrderBy", orderBy);

            var validator = ValidatorFactories[i](_standardsReadRepo.Object);
            var query = QueryFactories[i]("1", request);

            var result = await validator.TestValidateAsync(query);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor("OrderBy");
            else
            {
                result.ShouldHaveValidationErrorFor("OrderBy");
                result.Errors[0].ErrorMessage.Should().Be(Application.Courses.Queries.GetProvidersFromLarsCode.V1.GetProvidersForLarsCodeQueryValidator.OrderByRequiredErrorMessage);
            }
        }
    }

    [TestCase(null, true)]
    [TestCase(10, true)]
    [TestCase(0, false)]
    [TestCase(-1, false)]
    public async Task Validate_Distance_Works_For_V1_And_V2(int? distance, bool isValid)
    {
        for (var i = 0; i < ValidatorFactories.Length; i++)
        {
            var request = RequestFactories[i]();
            SetProperty(request, "Latitude", 80m);
            SetProperty(request, "Longitude", 0m);
            SetProperty(request, "OrderBy", ProviderOrderBy.Distance);
            SetProperty(request, "Distance", distance);

            var validator = ValidatorFactories[i](_standardsReadRepo.Object);
            var query = QueryFactories[i]("1", request);

            var result = await validator.TestValidateAsync(query);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor("Distance");
            else
            {
                result.ShouldHaveValidationErrorFor("Distance");
                result.Errors[0].ErrorMessage.Should().Be(Application.Courses.Queries.GetProvidersFromLarsCode.V1.GetProvidersForLarsCodeQueryValidator.DistanceErrorMessage);
            }
        }
    }

    private class WrapperValidator<T> : AbstractValidator<object>
    {
        public WrapperValidator(IValidator<T> inner)
        {
            RuleFor(x => (T)x).SetValidator(inner);
        }
    }

    private static void SetProperty(object target, string name, object value)
    {
        var prop = target.GetType().GetProperty(name);
        if (prop == null) return;
        var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        var converted = value == null ? null : Convert.ChangeType(value, propType);
        prop.SetValue(target, converted);
    }
}