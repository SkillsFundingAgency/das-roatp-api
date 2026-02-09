using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.ProvidersControllerTests;

public class ProvidersControllerV2Tests
{


    [Test, MoqAutoData]
    public async Task GetAllProviderCoursesV2_InvalidUkprn_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn)
    {
        var response = new ValidatedResponse<List<ProviderCourseModel>>(
            new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "error message",
                    PropertyName = "property name"
                }
            }
        );

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderCoursesQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        var result = await sut.GetAllProviderCoursesV2(ukprn);
        result.As<BadRequestObjectResult>().Should().NotBeNull();

        mediatorMock.Verify(a =>
                a.Send(It.Is<GetAllProviderCoursesQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task GetAllProviderCoursesV2_InvokesMediator_ReturnsCoursesDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn)
    {
        List<ProviderCourseModel> handlerResult = new List<ProviderCourseModel>();
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderCoursesQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<List<ProviderCourseModel>>(handlerResult));
        var result = await sut.GetAllProviderCoursesV2(ukprn);
        result.As<OkObjectResult>().Value.Should().BeEquivalentTo(handlerResult);
    }

    [Test, MoqAutoData]
    public async Task GetAllProviderCoursesV2_FiltersToApprenticeshipAndMapsToExternal(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn,
        ProviderCourseModel apprenticeship)
    {
        var shortCourse = new ProviderCourseModel
        {
            ProviderCourseId = 2,
            LarsCode = "999",
            CourseType = CourseType.ShortCourse
        };

        var handlerResult = new List<ProviderCourseModel> { apprenticeship, shortCourse };

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderCoursesQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<List<ProviderCourseModel>>(handlerResult));

        var actionResult = await sut.GetAllProviderCoursesV2(ukprn);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();

        var returned = ok.Value as IList<ProviderCourseModelExternal>;
        returned.Should().NotBeNull();
        returned.Should().HaveCount(handlerResult.Count);
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourse_CallsMediator_WithLarsCodeAsString(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn,
        string larsCode,
        ProviderCourseModel handlerResult)
    {
        handlerResult.LarsCode = larsCode.ToString();
        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<ProviderCourseModel>(handlerResult));
        var result = await sut.GetProviderCourse(ukprn, larsCode);
        var mappedResult = (ProviderCourseModelExternal)handlerResult;
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mappedResult);

        mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderCourseQuery>(t => t.Ukprn == ukprn && t.LarsCode == larsCode), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourse_InvalidUkprn_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn,
        int larsCode)
    {
        var response = new ValidatedResponse<ProviderCourseModel>(
            new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "error message",
                    PropertyName = "property name"
                }
            }
        );

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode.ToString()), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        var result = await sut.GetProviderCourse(ukprn, larsCode);
        result.As<BadRequestObjectResult>().Should().NotBeNull();

        mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderCourseQuery>(t => t.Ukprn == ukprn && t.LarsCode == larsCode.ToString()), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourse_V2_InvalidRequest_ReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProvidersController sut,
        int ukprn,
        string larsCode)
    {
        var response = new ValidatedResponse<ProviderCourseModel>(
            new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "error message",
                    PropertyName = "property name"
                }
            }
        );

        mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProviderCourseQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await sut.GetProviderCourse(ukprn, larsCode);

        result.As<BadRequestObjectResult>().Should().NotBeNull();

        mediatorMock.Verify(m =>
                m.Send(It.Is<GetProviderCourseQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}