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
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Application.Providers.Queries.GetRegisteredProvider;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
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
        public async Task GetAllProviderCourses_InvalidUkprn_ReturnsBadRequest(
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
            var result = await sut.GetAllProviderCourses(ukprn);
            result.As<BadRequestObjectResult>().Should().NotBeNull();

            mediatorMock.Verify(a =>
                    a.Send(It.Is<GetAllProviderCoursesQuery>(t => t.Ukprn == ukprn), It.IsAny<CancellationToken>()),
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

        [Test, MoqAutoData]
        public async Task GetAllProviderCourses_FiltersToApprenticeshipAndMapsToExternal(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn)
        {
            var apprenticeship = new ProviderCourseModel
            {
                ProviderCourseId = 1,
                LarsCode = "123",
                CourseType = CourseType.Apprenticeship,
                StandardInfoUrl = "https://standard.info",
                ContactUsEmail = "test@example.com",
                ContactUsPhoneNumber = "0123456789",
                IsApprovedByRegulator = true,
                IsImported = false,
                HasPortableFlexiJobOption = true,
                HasLocations = true,
                IsRegulatedForProvider = true,
                IfateReferenceNumber = "IFATE-1",
                Level = 3,
                CourseName = "Test Course",
                Version = "1.0",
                ApprovalBody = "Approver"
            };

            var shortCourse = new ProviderCourseModel
            {
                ProviderCourseId = 2,
                LarsCode = "999",
                CourseType = CourseType.ApprenticeshipUnit
            };

            var handlerResult = new List<ProviderCourseModel> { apprenticeship, shortCourse };

            mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderCoursesQuery>(q => q.Ukprn == ukprn), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidatedResponse<List<ProviderCourseModel>>(handlerResult));

            var actionResult = await sut.GetAllProviderCourses(ukprn);

            var ok = actionResult as OkObjectResult;
            ok.Should().NotBeNull();

            var returned = ok.Value as IList<ProviderCourseModelExternal>;
            returned.Should().NotBeNull();
            returned.Should().HaveCount(1);

            var mapped = returned!.First();
            mapped.ProviderCourseId.Should().Be(apprenticeship.ProviderCourseId);
            mapped.LarsCode.Should().Be(123); // parsed from string
            mapped.StandardInfoUrl.Should().Be(apprenticeship.StandardInfoUrl);
            mapped.ContactUsEmail.Should().Be(apprenticeship.ContactUsEmail);
            mapped.ContactUsPhoneNumber.Should().Be(apprenticeship.ContactUsPhoneNumber);
            mapped.IsApprovedByRegulator.Should().Be(apprenticeship.IsApprovedByRegulator);
            mapped.IsImported.Should().Be(apprenticeship.IsImported);
            mapped.HasPortableFlexiJobOption.Should().Be(apprenticeship.HasPortableFlexiJobOption);
            mapped.HasLocations.Should().Be(apprenticeship.HasLocations);
            mapped.IsRegulatedForProvider.Should().Be(apprenticeship.IsRegulatedForProvider);
            mapped.IfateReferenceNumber.Should().Be(apprenticeship.IfateReferenceNumber);
            mapped.Level.Should().Be(apprenticeship.Level);
            mapped.CourseName.Should().Be(apprenticeship.CourseName);
            mapped.Version.Should().Be(apprenticeship.Version);
            mapped.ApprovalBody.Should().Be(apprenticeship.ApprovalBody);
        }

        [Test, RecursiveMoqAutoData]
        public void ProviderCourseModel_ImplicitConversion_FromDomainEntity_SetsProperties(
            Domain.Entities.ProviderCourse providerCourse)
        {
            providerCourse.Id = 10;
            providerCourse.LarsCode = "555";
            providerCourse.StandardInfoUrl = "http://standard";
            providerCourse.ContactUsEmail = "a@b.c";
            providerCourse.ContactUsPhoneNumber = "000";
            providerCourse.IsApprovedByRegulator = true;
            providerCourse.IsImported = true;
            providerCourse.HasPortableFlexiJobOption = true;
            providerCourse.Locations = new List<ProviderCourseLocation> { new ProviderCourseLocation() };
            providerCourse.Standard = new Standard
            {
                CourseType = "Apprenticeship",
                IsRegulatedForProvider = true
            };

            var model = (ProviderCourseModel)providerCourse;

            model.Should().NotBeNull();
            model.ProviderCourseId.Should().Be(providerCourse.Id);
            model.LarsCode.Should().Be(providerCourse.LarsCode);
            model.StandardInfoUrl.Should().Be(providerCourse.StandardInfoUrl);
            model.ContactUsEmail.Should().Be(providerCourse.ContactUsEmail);
            model.ContactUsPhoneNumber.Should().Be(providerCourse.ContactUsPhoneNumber);
            model.IsApprovedByRegulator.Should().Be(providerCourse.IsApprovedByRegulator);
            model.IsImported.Should().Be(providerCourse.IsImported);
            model.HasPortableFlexiJobOption.Should().Be(providerCourse.HasPortableFlexiJobOption);
            model.HasLocations.Should().BeTrue(); // set from Locations.Count > 0
            model.IsRegulatedForProvider.Should().BeTrue(); // from Standard
            model.CourseType.Should().Be(CourseType.Apprenticeship); // parsed from Standard.CourseType string
        }

        [Test]
        public void ControllerConvention_HasRequiredNamespace()
        {
            var controllerPath = typeof(ProvidersController).Namespace.Split('.').Last();
            Assert.That(controllerPath == "ExternalReadControllers");
        }

        [Test, MoqAutoData]
        public async Task GetProviderCourse_CallsMediator_WithLarsCodeAsString(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn,
            int larsCode,
            ProviderCourseModel handlerResult)
        {
            handlerResult.LarsCode = larsCode.ToString();
            mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode.ToString()), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<ProviderCourseModel>(handlerResult));
            var result = await sut.GetProviderCourse(ukprn, larsCode);
            var mappedResult = (ProviderCourseModelExternal)handlerResult;
            (result as OkObjectResult).Value.Should().BeEquivalentTo(mappedResult);

            mediatorMock.Verify(a =>
                a.Send(It.Is<GetProviderCourseQuery>(t => t.Ukprn == ukprn && t.LarsCode == larsCode.ToString()), It.IsAny<CancellationToken>()),
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
    }
}