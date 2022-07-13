using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Queries;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse
{
    [TestFixture]
    public class ProviderAllCoursesQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            List<Domain.Entities.ProviderCourse> courses,
            [Frozen] Mock<IProviderCourseReadRepository> providerReadRepositoryMock,
            [Frozen] Mock<IStandardReadRepository> standardReadRepositoryMock,
            ProviderAllCoursesQuery query,
            ProviderAllCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerReadRepositoryMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(courses);
            var standards = courses.Select(course => new Standard { LarsCode = course.LarsCode }).ToList();
            standardReadRepositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);
            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Courses.Count, Is.EqualTo(courses.Count));
        }

        [Test, MoqAutoData()]
        public async Task Handle_NoData_ReturnsEmptyResult(
            [Frozen] Mock<IProviderCourseReadRepository> repoMock,
            ProviderAllCoursesQuery query,
            ProviderAllCoursesQueryHandler sut,
            CancellationToken cancellationToken)
        {
            repoMock.Setup(r => r.GetAllProviderCourses(query.Ukprn)).ReturnsAsync(new List<Domain.Entities.ProviderCourse>());
        
            var result = await sut.Handle(query, cancellationToken);
        
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Courses, Is.Empty);
        }
    }
}