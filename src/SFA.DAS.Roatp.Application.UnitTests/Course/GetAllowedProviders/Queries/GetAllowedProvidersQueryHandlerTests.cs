using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Course.GetAllowedProviders.Queries;

public class GetAllowedProvidersQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenCourseIsRestrictedAndProviderExistsInProviderCourseTypes_ThenReturnsProvidersFromProviderAllowedCourse(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
            [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
            [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
            [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
            GetAllowedProvidersQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = courseType,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date
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

        var providerCourseTypes = new List<ProviderCourseType>
            {
                new()
                {
                    Ukprn = 100001,
                    CourseType = courseType
                }
            };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        restrictedCourseViewRepository.Setup(x => x.GetRestrictedCourses(cancellationToken)).ReturnsAsync(new List<RestrictedCourseView> { new() { LarsCode = larsCode } });

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProviderCourseTypes(cancellationToken)).ReturnsAsync(providerCourseTypes);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Result.Should().NotBeNull();
        response.Result.LarsCode.Should().Be(standard.LarsCode);
        response.Result.IfateReferenceNumber.Should().Be(standard.IfateReferenceNumber);
        response.Result.CourseName.Should().Be(standard.Title);
        response.Result.Route.Should().Be(standard.Route);
        response.Result.LearningType.Should().Be(standard.LearningType);
        response.Result.CourseType.Should().Be(standard.CourseType);
        response.Result.IsActiveAvailable.Should().Be(standard.IsActiveAvailable);
        response.Result.DateLastStarts.Should().Be(standard.LastDateStarts);
        response.Result.IsCourseRestricted.Should().BeTrue();
        response.Result.Providers[0].Ukprn.Should().Be(providerAllowedCourses[0].Ukprn);
        response.Result.Providers[0].ProviderName.Should().Be(providerAllowedCourses[0].Provider.LegalName);
        response.Result.Providers[0].DateLastStarts.Should().Be(providerAllowedCourses[0].LastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenCourseIsNotRestrictedAndProviderExistsInProviderCourseTypes_ThenReturnsProvidersFromProviderCourse(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
        GetAllowedProvidersQueryHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = courseType,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new()
                {
                    Ukprn = 100001,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        var providerCourseTypes = new List<ProviderCourseType>
            {
                new()
                {
                    Ukprn = 100001,
                    CourseType = courseType
                }
            };

        var providerCourses = new List<SFA.DAS.Roatp.Domain.Entities.ProviderCourse>
            {
                new()
                {
                    Provider = new Provider { Ukprn = 100001, LegalName = "TestProvider" }
                }
            };


        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        restrictedCourseViewRepository.Setup(x => x.GetRestrictedCourses(cancellationToken)).ReturnsAsync(new List<RestrictedCourseView>());

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCoursesReadRepository.Setup(x => x.GetProviderCoursesByLarsCode(larsCode)).ReturnsAsync(providerCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProviderCourseTypes(cancellationToken)).ReturnsAsync(providerCourseTypes);

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Result.Should().NotBeNull();
        response.Result.LarsCode.Should().Be(standard.LarsCode);
        response.Result.IfateReferenceNumber.Should().Be(standard.IfateReferenceNumber);
        response.Result.CourseName.Should().Be(standard.Title);
        response.Result.Route.Should().Be(standard.Route);
        response.Result.LearningType.Should().Be(standard.LearningType);
        response.Result.CourseType.Should().Be(standard.CourseType);
        response.Result.IsActiveAvailable.Should().Be(standard.IsActiveAvailable);
        response.Result.DateLastStarts.Should().Be(standard.LastDateStarts);
        response.Result.IsCourseRestricted.Should().BeFalse();
        response.Result.Providers[0].Ukprn.Should().Be(providerCourses[0].Provider.Ukprn);
        response.Result.Providers[0].ProviderName.Should().Be(providerCourses[0].Provider.LegalName);
        response.Result.Providers[0].DateLastStarts.Should().Be(providerAllowedCourses[0].LastDateStarts);
    }

    [Test, MoqAutoData]
    public async Task WhenProviderDoesNotExistsInProviderCourseTypes_ThenProviderIsNotReturned(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Frozen] Mock<IRestrictedCourseViewRepository> restrictedCourseViewRepository,
        [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
        [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepository,
        [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
        GetAllowedProvidersQueryHandler sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var courseType = CourseType.Apprenticeship;

        var standard = new Standard
        {
            LarsCode = larsCode,
            IfateReferenceNumber = "TestIfate",
            Title = "TestTitle",
            Route = "TestRoute",
            LearningType = LearningType.Apprenticeship,
            CourseType = courseType,
            IsActiveAvailable = true,
            LastDateStarts = DateTime.UtcNow.Date
        };

        var providerAllowedCourses = new List<ProviderAllowedCourse>
            {
                new()
                {
                    Ukprn = 100001,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        restrictedCourseViewRepository.Setup(x => x.GetRestrictedCourses(cancellationToken)).ReturnsAsync(new List<RestrictedCourseView> { new() { LarsCode = larsCode } });

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProviderCourseTypes(cancellationToken)).ReturnsAsync(new List<ProviderCourseType>());

        // Act
        var response = await sut.Handle(new GetAllowedProvidersQuery(larsCode), cancellationToken);

        // Assert
        response.Result.Providers.Should().BeEmpty();
    }
}
