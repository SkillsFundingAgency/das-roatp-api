﻿using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers
{
    public class ProvidersControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetProviders_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
            )
        {
            GetProvidersQueryResult handlerResult = new GetProvidersQueryResult();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);
            var result = await sut.GetProviders();
            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetProvider_CallsMediator(
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut,
           int ukprn,
           GetProviderSummaryQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProviderSummaryQueryResult>(handlerResult));
            var result = await sut.GetProvider(ukprn);
            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetProvider_ProviderDoesNotExist_ReturnsNotFound(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn)
        {
            var handlerResult = (GetProviderSummaryQueryResult)null;
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetProviderSummaryQueryResult>(handlerResult));
            var result = await sut.GetProvider(ukprn);
            result.As<NotFoundResult>().Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task GetProvider_InvalidUkprn_ReturnsBadRequest(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn)
        {
            var response = new ValidatedResponse<GetProviderSummaryQueryResult>(new List<ValidationFailure>
            {
                new()
                {
                    ErrorMessage = "error message",
                    PropertyName = "property name"
                }
            });

            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
            var result = await sut.GetProvider(ukprn);
            result.As<BadRequestObjectResult>().Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task GetAllProviderCourses_InvokesMediator_ReturnsCoursesDetails(
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut,
           int ukprn,
           List<ProviderCourseModel> handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderCoursesQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<List<ProviderCourseModel>>(handlerResult));
            var result = await sut.GetAllProviderCourses(ukprn);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test]
        public void ControllerConvention_HasRequiredNamespace()
        {
            var controllerPath = typeof(ProvidersController).Namespace.Split('.').Last();
            Assert.That(controllerPath == "ExternalReadControllers");
        }

        [Test, MoqAutoData]
        public async Task GetCourse_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn,
            int larsCode,
            ProviderCourseModel handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<ProviderCourseModel>(handlerResult));
            var result = await sut.GetProviderCourse(ukprn, larsCode);
            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
