using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetAllowedProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetAllowedProviders;

public class GetAllowedProvidersQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenStandardDoesNotExist_ThenReturnsNull(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            GetAllowedProvidersQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync((Standard)null);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsRestricted_ThenReturnsProvidersFromProviderAllowedCourse(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
            [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
            GetAllowedProvidersQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = CourseType.Apprenticeship,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date,
            RestrictedCourseView = new RestrictedCourseView { LarsCode = larsCode }
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new()
                {
                    Ukprn = 100001,
                    LastDateStarts = DateTime.UtcNow.Date,
                    Provider = new Provider { LegalName = "TestProvider" }
                }
            };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.LarsCode.Should().Be(standard.LarsCode);
        response.IfateReferenceNumber.Should().Be(standard.IfateReferenceNumber);
        response.CourseName.Should().Be(standard.Title);
        response.Route.Should().Be(standard.Route);
        response.LearningType.Should().Be(standard.LearningType);
        response.CourseType.Should().Be(standard.CourseType);
        response.IsActiveAvailable.Should().Be(standard.IsActiveAvailable);
        response.DateLastStarts.Should().Be(standard.LastDateStarts);
        response.IsCourseRestricted.Should().BeTrue();
        response.Providers[0].Ukprn.Should().Be(providerAllowedCourses[0].Ukprn);
        response.Providers[0].ProviderName.Should().Be(providerAllowedCourses[0].Provider.LegalName);
        response.Providers[0].DateLastStarts.Should().Be(providerAllowedCourses[0].LastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsNotRestricted_ThenReturnsProvidersFromProviderCourse(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetAllowedProvidersQueryHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = CourseType.Apprenticeship,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date,
            RestrictedCourseView = null
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new()
                {
                    Ukprn = 100001,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        var providerCourses = new List<Domain.Entities.ProviderCourse>
            {
                new()
                {
                    Provider = new Provider { Ukprn = 100001, LegalName = "TestProvider" }
                }
            };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCoursesReadRepository.Setup(x => x.GetProviderCoursesByLarsCode(larsCode)).ReturnsAsync(providerCourses);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.LarsCode.Should().Be(standard.LarsCode);
        response.IfateReferenceNumber.Should().Be(standard.IfateReferenceNumber);
        response.CourseName.Should().Be(standard.Title);
        response.Route.Should().Be(standard.Route);
        response.LearningType.Should().Be(standard.LearningType);
        response.CourseType.Should().Be(standard.CourseType);
        response.IsActiveAvailable.Should().Be(standard.IsActiveAvailable);
        response.DateLastStarts.Should().Be(standard.LastDateStarts);
        response.IsCourseRestricted.Should().BeFalse();
        response.Providers[0].Ukprn.Should().Be(providerCourses[0].Provider.Ukprn);
        response.Providers[0].ProviderName.Should().Be(providerCourses[0].Provider.LegalName);
        response.Providers[0].DateLastStarts.Should().Be(providerAllowedCourses[0].LastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsNotRestrictedAndNoDataInProviderAllowedCourse_VerifyDateLastStartsForProviderIsNull(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        GetAllowedProvidersQueryHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = CourseType.Apprenticeship,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date,
            RestrictedCourseView = null
        };

        var providerCourses = new List<Domain.Entities.ProviderCourse>
            {
                new()
                {
                    Provider = new Provider { Ukprn = 100001, LegalName = "TestProvider" }
                }
            };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(new List<ProviderAllowedCourse>());

        providerCoursesReadRepository.Setup(x => x.GetProviderCoursesByLarsCode(larsCode)).ReturnsAsync(providerCourses);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Providers[0].DateLastStarts.Should().BeNull();
    }
}
