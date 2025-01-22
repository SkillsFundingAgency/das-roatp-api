using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourses;

public sealed class GetCoursesQueryHandlerTests
{
    [Test]
    [RecursiveMoqAutoData()]
    public async Task Handle_Returns_ListOfCourses(
        [Frozen] Mock<IProvidersCountReadRepository> trainingCoursesReadRepository,
        GetCourseTrainingProvidersCountQuery query,
        GetCourseTrainingProvidersCountQueryHandler sut,
        CourseInformation course,
        CancellationToken cancellationToken
    )
    {
        var repositoryResponse = new List<CourseInformation>() { course };

        trainingCoursesReadRepository.Setup(r => 
            r.GetProviderTrainingCourses(
                query.LarsCodes,
                query.Longitude,
                query.Latitude,
                query.Distance,
                cancellationToken
            )
        ).ReturnsAsync(repositoryResponse);

        var response = await sut.Handle(query, cancellationToken);

        var result = response.Result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Courses.Count, Is.EqualTo(1));

            var resultCourse = result.Courses[0];
            Assert.That(resultCourse.LarsCode, Is.EqualTo(course.LarsCode));
            Assert.That(resultCourse.ProvidersCount, Is.EqualTo(course.ProvidersCount));
            Assert.That(resultCourse.TotalProvidersCount, Is.EqualTo(course.TotalProvidersCount));
        });
    }
}
