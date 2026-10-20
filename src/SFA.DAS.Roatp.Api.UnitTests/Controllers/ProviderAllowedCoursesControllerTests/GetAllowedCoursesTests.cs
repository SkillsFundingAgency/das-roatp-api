using System.Threading;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderAllowedCoursesControllerTests;

public class GetAllowedCoursesTests
{
    [Test, MoqAutoData]
    public void WhenUkprnIsValid_ThenReturnsOkResult(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Frozen] Mock<IValidator<IUkprn>> _ukprnValidatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        CourseType courseType,
        GetProviderAllowedCoursesQueryResult expected)
    {
        _ukprnValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IUkprn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mediatorMock.Setup(m => m.Send(It.Is<GetProviderAllowedCoursesQuery>(q => q.Ukprn == ukprn && q.CourseType == courseType), CancellationToken.None))
            .ReturnsAsync(expected);

        var result = sut.GetAllowedCourses(ukprn, courseType, CancellationToken.None).GetAwaiter().GetResult();

        result.As<OkObjectResult>().Value.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public void WhenUkprnIsNotValid_ReturnsNotFound(
        [Frozen] Mock<IValidator<IUkprn>> _ukprnValidatorMock,
        [Greedy] ProviderAllowedCoursesController sut,
        int ukprn,
        CourseType courseType)
    {
        _ukprnValidatorMock
            .Setup(v => v.ValidateAsync(It.Is<IUkprn>(x => x.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(
            [
                new ValidationFailure(
                    nameof(IUkprn.Ukprn),
                    UkprnValidator.InvalidUkprnErrorMessage)
            ]));

        var result = sut.GetAllowedCourses(ukprn, courseType, CancellationToken.None).GetAwaiter().GetResult();

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
