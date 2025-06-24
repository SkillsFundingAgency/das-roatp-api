using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.AutoMoq;
using AutoFixture;
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
            GetAllProviderCoursesQuery query,
            GetAllProviderCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);
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
            int expectedCoursesCount
            //Domain.Entities.ProviderCourse course,
            //[Frozen] Mock<IProviderCoursesReadRepository> providersReadRepositoryMock,
            //[Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            //GetAllProviderCoursesQuery query,
            //GetAllProviderCoursesQueryHandler sut,
            //CancellationToken cancellationToken
            )
        {
            //Domain.Entities.ProviderCourse course = new Domain.Entities.ProviderCourse();
            //course.Standard.IsRegulatedForProvider = isRegulatedForProvider;
            //course.IsApprovedByRegulator = isApprovedByRegulator;
            //List<Domain.Entities.ProviderCourse> courses = new List<Domain.Entities.ProviderCourse> { course };

            //providersReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            //var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            //standardsReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);
            //var response = await sut.Handle(query, cancellationToken);

            //response.Should().NotBe(null);
            //response.Result.Count.Should().Be(expectedCoursesCount);

            // Create and configure AutoFixture with Moq
            var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

            // Freeze mocks so that the same instance is injected wherever needed
            var providersReadRepositoryMock = fixture.Freeze<Mock<IProviderCoursesReadRepository>>();
            var standardsReadRepositoryMock = fixture.Freeze<Mock<IStandardsReadRepository>>();

            // Generate other objects
            var query = fixture.Create<GetAllProviderCoursesQuery>();
            var cancellationToken = CancellationToken.None;

            // Create a course with the test-case-specific values
            var course = new Domain.Entities.ProviderCourse
            {
                Standard = new Domain.Entities.Standard
                {
                    IsRegulatedForProvider = isRegulatedForProvider
                },
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var courses = new List<Domain.Entities.ProviderCourse> { course };

            // Mock the repository responses
            providersReadRepositoryMock
                .Setup(r => r.GetAllProviderCourses(query.Ukprn))
                .ReturnsAsync(courses);

            standardsReadRepositoryMock
                .Setup(r => r.GetAllStandards())
                .ReturnsAsync(courses.Select(c => new Standard { LarsCode = c.LarsCode }).ToList());

            // Create the SUT (query handler) — AutoFixture will inject frozen mocks
            var sut = fixture.Create<GetAllProviderCoursesQueryHandler>();

            // Act
            var response = await sut.Handle(query, cancellationToken);

            // Assert
            response.Should().NotBeNull();
            response.Result.Count.Should().Be(expectedCoursesCount);
        }
    }
}