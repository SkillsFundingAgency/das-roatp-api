using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries
{
    [TestFixture]
    public class ProviderAllCoursesQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_ReturnsResult(
            List<Domain.Entities.ProviderCourse> courses,
            [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            GetAllProviderCoursesQuery Query = new GetAllProviderCoursesQuery(1, true, null);
            var larsCodeIncrement = 1;
            foreach (var course in courses)
            {
                course.ProviderId = 1;
                course.LarsCode = larsCodeIncrement.ToString();
                larsCodeIncrement++;

            }
            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(Query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            var response = await sut.Handle(Query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(courses.Count);
        }

        [Test, MoqAutoData]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IProviderCoursesReadRepository> repoMock,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            GetAllProviderCoursesQuery Query = new GetAllProviderCoursesQuery(1, true, null);

            repoMock.Setup(r => r.GetAllProviderCourses(Query.Ukprn)).ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

            var response = await sut.Handle(Query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Should().BeEmpty();
        }


        [TestCase(false, false, 1)]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 1)]
        [TestCase(true, false, 0)]
        public async Task Handle_UnapprovedRegulatedStandard_RemovedFromResult(
            bool isRegulatedForProvider,
            bool isApprovedByRegulator,
            int expectedCoursesCount)
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            var providersReadRepositoryMock = fixture.Freeze<Mock<IProviderCoursesReadRepository>>();
            var standardsReadRepositoryMock = fixture.Freeze<Mock<IStandardsReadRepository>>();

            GetAllProviderCoursesQuery Query = new GetAllProviderCoursesQuery(1, true, null);

            var cancellationToken = CancellationToken.None;

            var course = new Domain.Entities.ProviderCourse
            {
                Standard = new Standard
                {
                    IsRegulatedForProvider = isRegulatedForProvider
                },
                IsApprovedByRegulator = isApprovedByRegulator,
                ProviderId = 1,
                LarsCode = "1",
                Locations = [new()]
            };

            var courses = new List<Domain.Entities.ProviderCourse> { course };

            providersReadRepositoryMock
                .Setup(r => r.GetAllProviderCourses(Query.Ukprn))
                .ReturnsAsync(courses);

            standardsReadRepositoryMock
                .Setup(r => r.GetAllStandards())
                .ReturnsAsync(courses.Select(c => new Standard { LarsCode = c.LarsCode }).ToList());

            var sut = fixture.Create<GetAllProviderCoursesQueryHandler>();

            var response = await sut.Handle(Query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(expectedCoursesCount);
        }

        [Test, MoqAutoData]
        public async Task Handle_StandardsWithoutLocation_RemovedFromResultWhenExcludeCoursesWithoutLocation(
            [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            string larsCodeOne = "1";
            string larsCodeTwo = "2";

            GetAllProviderCoursesQuery Query = new GetAllProviderCoursesQuery(1, true, null);

            var courses = new List<Domain.Entities.ProviderCourse>
            {
                new() { ProviderId = 1, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false }, LarsCode = larsCodeOne, Locations = [new ProviderCourseLocation()] },
                new() { ProviderId = 2, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false }, LarsCode = larsCodeTwo }
            };

            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(Query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            var response = await sut.Handle(Query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(1);
        }

        [Test, MoqAutoData]
        public async Task Handle_StandardsWithoutLocation_NotRemovedFromResultWhenNotExcludeCoursesWithoutLocation(
        [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,

        GetAllProviderCoursesQueryHandler sut,
        CancellationToken cancellationToken)
        {

            string larsCodeOne = "1";
            string larsCodeTwo = "2";

            GetAllProviderCoursesQuery Query = new GetAllProviderCoursesQuery(1, false, null);

            var courses = new List<Domain.Entities.ProviderCourse>
            {
                new() { ProviderId = 1, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false }, LarsCode = larsCodeOne },
                new() { ProviderId = 2, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false }, LarsCode = larsCodeTwo }
            };

            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(Query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            var response = await sut.Handle(Query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(2);
        }

        [Test, MoqAutoData]
        public async Task Handle_FiltersByCourseType_ReturnsOnlyMatching(
            [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            var larsCodeOne = "1";
            var larsCodeTwo = "2";

            var courseTypeApprenticeship = CourseType.Apprenticeship;
            var courseTypeApprenticeshipUnit = CourseType.ApprenticeshipUnit;

            var query = new GetAllProviderCoursesQuery(1, false, courseTypeApprenticeship);

            var courses = new List<Domain.Entities.ProviderCourse>
            {
                new() { ProviderId = 1, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false, CourseType = "Apprenticeship" }, LarsCode = larsCodeOne},
                new() { ProviderId = 1, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false, CourseType = "ApprenticeshipUnit" }, LarsCode = larsCodeTwo}
            };

            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(1);
            response.Result.All(r => r.CourseType == courseTypeApprenticeship).Should().BeTrue();

            query = new GetAllProviderCoursesQuery(1, false, courseTypeApprenticeshipUnit);

            response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(1);
            response.Result.All(r => r.CourseType == courseTypeApprenticeshipUnit).Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Handle_NoCourseTypeFilter_ReturnsBoth(
            [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            // Arrange
            const int ukprn = 1;
            var query = new GetAllProviderCoursesQuery(ukprn, false, null);

            var courses = new List<Domain.Entities.ProviderCourse>
            {
                new() { ProviderId = ukprn, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false, CourseType = "Apprenticeship" }, LarsCode = "1" },
                new() { ProviderId = ukprn, IsApprovedByRegulator = true, Standard = new Standard { IsRegulatedForProvider = false, CourseType = "ApprenticeshipUnit" }, LarsCode = "2" }
            };

            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            // Act
            var response = await sut.Handle(query, cancellationToken);

            // Assert
            response.Should().NotBeNull();
            response.Result.Should().HaveCount(2);
            response.Result.Should().ContainSingle(r => r.CourseType == CourseType.Apprenticeship);
            response.Result.Should().ContainSingle(r => r.CourseType == CourseType.ApprenticeshipUnit);
        }
    }
}