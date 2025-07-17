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
            [Frozen] Mock<IProviderCourseLocationsReadRepository> providerCourseLocationsReadRepositoryMock,
            GetAllProviderCoursesQuery query,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            foreach (var course in courses)
            {
                course.ProviderId = 1;
            }
            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            List<ProviderCourseLocation> providerLocations =
                [new()];
            providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(query.Ukprn, It.IsAny<int>()))
                .ReturnsAsync(providerLocations);

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(courses.Count);
        }

        [Test, MoqAutoData]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IProviderCoursesReadRepository> repoMock,
            GetAllProviderCoursesQuery query,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(new List<Domain.Entities.ProviderCourse>());

            var response = await sut.Handle(query, cancellationToken);

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
            var providerCourseLocationsReadRepositoryMock = fixture.Freeze<Mock<IProviderCourseLocationsReadRepository>>();

            var query = fixture.Create<GetAllProviderCoursesQuery>();
            var cancellationToken = CancellationToken.None;

            var course = new Domain.Entities.ProviderCourse
            {
                Standard = new Standard
                {
                    IsRegulatedForProvider = isRegulatedForProvider
                },
                IsApprovedByRegulator = isApprovedByRegulator,
                ProviderId = 1
            };

            var courses = new List<Domain.Entities.ProviderCourse> { course };

            providersReadRepositoryMock
                .Setup(r => r.GetAllProviderCourses(query.Ukprn))
                .ReturnsAsync(courses);

            standardsReadRepositoryMock
                .Setup(r => r.GetAllStandards())
                .ReturnsAsync(courses.Select(c => new Standard { LarsCode = c.LarsCode }).ToList());

            var providerLocations = new List<ProviderCourseLocation> { new() };

            providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(query.Ukprn, It.IsAny<int>()))
                .ReturnsAsync(providerLocations);

            var sut = fixture.Create<GetAllProviderCoursesQueryHandler>();

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(expectedCoursesCount);
        }

        [Test, MoqAutoData]
        public async Task Handle_StandardsWithoutLocation_RemovedFromResult(
            [Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            [Frozen] Mock<IProviderCourseLocationsReadRepository> providerCourseLocationsReadRepositoryMock,
            int larsCodeOne,
            int larsCodeTwo,
            GetAllProviderCoursesQuery query,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            List<Domain.Entities.ProviderCourse> courses =
            [
                new() { ProviderId = 1, IsApprovedByRegulator = true, Standard = new Standard{IsRegulatedForProvider = false}, LarsCode = larsCodeOne},
                new() { ProviderId = 2, IsApprovedByRegulator = true, Standard = new Standard{IsRegulatedForProvider = false}, LarsCode = larsCodeTwo}
            ];
            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);

            List<ProviderCourseLocation> providerLocations =
                [new()];
            providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(query.Ukprn, larsCodeOne))
                .ReturnsAsync(providerLocations);
            providerCourseLocationsReadRepositoryMock
                .Setup(r => r.GetAllProviderCourseLocations(query.Ukprn, larsCodeTwo))
                .ReturnsAsync(new List<ProviderCourseLocation>());

            var response = await sut.Handle(query, cancellationToken);

            response.Should().NotBeNull();
            response.Result.Count.Should().Be(1);
        }
    }
}