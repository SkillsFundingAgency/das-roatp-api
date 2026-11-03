using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAvailableCourses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

public class ProviderAvailableCoursesControllerTests
{
    [Test, AutoData]
    public async Task WhenGettingAvailableCourses_InvokesMediator_ReturnsOk(int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderAvailableCoursesQuery>(), cancellationToken)).ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>(new GetProviderAvailableCoursesQueryResult()));
        ProviderAvailableCoursesController sut = new(mediatorMock.Object);

        var response = await sut.GetAvailableCourses(ukprn, courseType, cancellationToken);

        mediatorMock.Verify(m => m.Send(It.Is<GetProviderAvailableCoursesQuery>(q => q.Ukprn == ukprn && q.CourseType == courseType), cancellationToken));
        Assert.That(response, Is.InstanceOf<OkObjectResult>());
    }

    [Test, AutoData]
    public async Task WhenGettingAvailableCourses_AndValidationFails_ReturnsBadRequest(int ukprn, CourseType courseType, CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderAvailableCoursesQuery>(), cancellationToken)).ReturnsAsync(new ValidatedResponse<GetProviderAvailableCoursesQueryResult>([new ValidationFailure("Property", "Error message")]));
        ProviderAvailableCoursesController sut = new(mediatorMock.Object);

        var response = await sut.GetAvailableCourses(ukprn, courseType, cancellationToken);

        mediatorMock.Verify(m => m.Send(It.Is<GetProviderAvailableCoursesQuery>(q => q.Ukprn == ukprn && q.CourseType == courseType), cancellationToken));
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test, AutoData]
    public async Task WhenGettingAvailableCourse_AndResultIsNotNull_ReturnsOk(
     int ukprn,
     CourseType courseType,
     string larsCode,
     CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderAvailableCourseQuery>(), cancellationToken))
            .ReturnsAsync(new GetProviderAvailableCourseQueryResult
            {
                LarsCode = larsCode
            });

        ProviderAvailableCoursesController sut = new(mediatorMock.Object);

        var response = await sut.GetAvailableCourse(ukprn, courseType, larsCode, cancellationToken);

        Assert.That(response, Is.InstanceOf<OkObjectResult>());
    }

    [Test, AutoData]
    public async Task WhenGettingAvailableCourse_AndResultIsNull_ReturnsNotFound(
        int ukprn,
        CourseType courseType,
        string larsCode,
        CancellationToken cancellationToken)
    {
        Mock<IMediator> mediatorMock = new();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderAvailableCourseQuery>(), cancellationToken))
            .ReturnsAsync((GetProviderAvailableCourseQueryResult)null!);

        ProviderAvailableCoursesController sut = new(mediatorMock.Object);

        var response = await sut.GetAvailableCourse(ukprn, courseType, larsCode, cancellationToken);

        Assert.That(response, Is.InstanceOf<NotFoundResult>());
    }
}
