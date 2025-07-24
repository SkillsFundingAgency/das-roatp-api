using System.Collections.Generic;
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
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers
{
    public class ProvidersControllerTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetProviders_CallsMediator_WithLiveEqualsFalse(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
        )
        {
            GetProvidersQueryResult handlerResult = new GetProvidersQueryResult();
            mediatorMock.Setup(m =>
                m.Send(
                    It.Is<GetProvidersQuery>(a => a.Live.Equals(false)),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(handlerResult);

            var result = await sut.GetProviders(false, CancellationToken.None);

            mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetProvidersQuery>(a => a.Live.Equals(false)),
                    It.IsAny<CancellationToken>()
                )
            , Times.Once);

            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test]
        [MoqAutoData]
        public async Task GetProviders_CallsMediator_WithLiveEqualsTrue(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
        )
        {
            GetProvidersQueryResult handlerResult = new GetProvidersQueryResult();
            mediatorMock.Setup(m =>
                m.Send(
                    It.Is<GetProvidersQuery>(a => a.Live.Equals(true)),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(handlerResult);

            var result = await sut.GetProviders(true, CancellationToken.None);

            mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetProvidersQuery>(a => a.Live.Equals(true)),
                    It.IsAny<CancellationToken>()
                )
            , Times.Once);

            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetProviders_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
            )
        {
            GetProvidersQueryResult handlerResult = new GetProvidersQueryResult();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);
            var result = await sut.GetProviders(false, CancellationToken.None);
            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetProviderSummary_CallsMediator_WithRequestedUkprn(
           int ukprn,
           GetProviderSummaryQueryResult handlerResult,
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut
        )
        {
            mediatorMock.Setup(m =>
                m.Send(It.IsAny<GetProviderSummaryQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ValidatedResponse<GetProviderSummaryQueryResult>(handlerResult));

            var result = await sut.GetProviderSummary(ukprn);

            Assert.That(result, Is.Not.Null);

            var convertedResult = (result as OkObjectResult).Value as GetProviderSummaryQueryResult;

            Assert.That(convertedResult, Is.EqualTo(handlerResult));

            mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderSummaryQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Test, MoqAutoData]
        public async Task GetRegisteredProvider_CallsMediator_WithRequestedUkprn(
           int ukprn,
           GetRegisteredProviderQueryResult expectedResult,
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut,
           CancellationToken cancellationToken
        )
        {
            mediatorMock.Setup(m =>
                m.Send(It.IsAny<GetRegisteredProviderQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ValidatedResponse<GetRegisteredProviderQueryResult>(expectedResult));

            var actualResult = await sut.GetRegisteredProvider(ukprn, cancellationToken);

            actualResult.As<OkObjectResult>().Should().NotBeNull();
            actualResult.As<OkObjectResult>().Value.As<GetRegisteredProviderQueryResult>().Should().Be(expectedResult);

            mediatorMock.Verify(a =>
                a.Send(It.Is<GetRegisteredProviderQuery>(t => t.Ukprn == ukprn), cancellationToken),
                Times.Once
            );
        }

        [Test, MoqAutoData]
        public async Task GetProviderSummary_ProviderDoesNotExist_ReturnsNotFound(
            int ukprn,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
        )
        {
            mediatorMock.Setup(m =>
                m.Send(
                    It.IsAny<GetProviderSummaryQuery>(),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(
                new ValidatedResponse<GetProviderSummaryQueryResult>((GetProviderSummaryQueryResult)null)
            );

            var result = await sut.GetProviderSummary(ukprn);

            result.As<NotFoundResult>().Should().NotBeNull();

            mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderSummaryQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Test, MoqAutoData]
        public async Task GetProviderSummary_InvalidUkprn_ReturnsBadRequest(
            int ukprn,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
        )
        {
            var response = new ValidatedResponse<GetProviderSummaryQueryResult>(
                new List<ValidationFailure>
                {
                    new()
                    {
                        ErrorMessage = "error message",
                        PropertyName = "property name"
                    }
                }
            );

            mediatorMock.Setup(m =>
                m.Send(
                    It.IsAny<GetProviderSummaryQuery>(),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(response);

            var result = await sut.GetProviderSummary(ukprn);

            result.As<BadRequestObjectResult>().Should().NotBeNull();

            mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderSummaryQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Test, MoqAutoData]
        public async Task GetAllProviderCourses_InvokesMediator_ReturnsCoursesDetails(
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut,
           int ukprn)
        {
            List<ProviderCourseModel> handlerResult = new List<ProviderCourseModel>();
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
            int larsCode)
        {
            ProviderCourseModel handlerResult = new ProviderCourseModel();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<ProviderCourseModel>(handlerResult));
            var result = await sut.GetProviderCourse(ukprn, larsCode);
            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
