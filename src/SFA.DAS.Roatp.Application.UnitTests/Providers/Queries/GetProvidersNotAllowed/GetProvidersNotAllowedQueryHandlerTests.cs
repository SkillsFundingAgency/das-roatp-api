using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvidersNotAllowed;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProvidersNotAllowed;

public class GetProvidersNotAllowedQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenStandardDoesNotExist_ThenReturnsNull(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            GetProvidersNotAllowedQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync((Standard)null);

        // Act
        var response = await sut.Handle(new GetProvidersNotAllowedQuery(larsCode), cancellationToken);

        // Assert
        response.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenProviderHasCourseType_AndNoMatchingLarsCodeInProviderAllowedCourse_ThenReturnsProvide(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
            [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
            GetProvidersNotAllowedQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var ukprn = 100001;
        var nonMatchingLarsCode = "567689";

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
                    Ukprn = ukprn,
                    LarsCode = nonMatchingLarsCode,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        var providers = new List<ProviderCourseType>
        {
            new()
            {
                Ukprn = ukprn,
                CourseType = standard.CourseType,
                Provider = new Provider { LegalName = "TestProvider", Ukprn = ukprn }
            }
        };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProvidersByCourseType(standard.CourseType, cancellationToken)).ReturnsAsync(providers);

        // Act
        var response = await sut.Handle(new GetProvidersNotAllowedQuery(larsCode), cancellationToken);

        // Assert
        using (new AssertionScope())
        {
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
            response.Providers[0].Ukprn.Should().Be(providers[0].Ukprn);
            response.Providers[0].ProviderName.Should().Be(providers[0].Provider.LegalName);
            response.Providers[0].DateLastStarts.Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public async Task WhenProviderHasCourseTypeAndCourseIsAllowed_ThenDoesNotReturnProvider(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
            [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
            GetProvidersNotAllowedQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var ukprn = 100001;

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
                    Ukprn = ukprn,
                    LarsCode = larsCode,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        var providers = new List<ProviderCourseType>
        {
            new()
            {
                Ukprn = ukprn,
                CourseType = standard.CourseType,
                Provider = new Provider { LegalName = "TestProvider", Ukprn = ukprn }
            }
        };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProvidersByCourseType(standard.CourseType, cancellationToken)).ReturnsAsync(providers);

        // Act
        var response = await sut.Handle(new GetProvidersNotAllowedQuery(larsCode), cancellationToken);

        // Assert
        response.Providers.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public async Task WhenProviderDoesNotHaveCourseType_AndCourseIsAllowed_ThenDoesNotReturnProvider(
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
            [Frozen] Mock<IProviderAllowedCoursesRepository> providerAllowedCoursesRepository,
            [Frozen] Mock<IProviderCourseTypesReadRepository> providerCourseTypesReadRepository,
            GetProvidersNotAllowedQueryHandler sut,
            CancellationToken cancellationToken)
    {
        // Arrange
        var larsCode = "123456";
        var ukprn = 100001;

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
                    Ukprn = ukprn,
                    LarsCode = larsCode,
                    LastDateStarts = DateTime.UtcNow.Date
                }
            };

        var providers = new List<ProviderCourseType>
        {
            new()
            {
                Ukprn = ukprn,
                CourseType = CourseType.ShortCourse,
                Provider = new Provider { LegalName = "TestProvider", Ukprn = ukprn }
            }
        };

        standardsReadRepository.Setup(x => x.GetStandard(larsCode)).ReturnsAsync(standard);

        providerAllowedCoursesRepository.Setup(x => x.GetProviderAllowedCoursesByLarsCode(larsCode, cancellationToken)).ReturnsAsync(providerAllowedCourses);

        providerCourseTypesReadRepository.Setup(x => x.GetAllProvidersByCourseType(standard.CourseType, cancellationToken)).ReturnsAsync(providers); ;

        // Act
        var response = await sut.Handle(new GetProvidersNotAllowedQuery(larsCode), cancellationToken);

        // Assert
        response.Providers.Should().BeEmpty();
    }
}