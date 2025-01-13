using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        [Test, RecursiveMoqAutoData()]
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

        [Test, MoqAutoData()]
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
    }
}