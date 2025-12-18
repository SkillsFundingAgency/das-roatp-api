using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseTrainingProvidersCount;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

public sealed class CoursesCountControllerTests
{
    [Test]
    [MoqAutoData]
    public async Task GetTrainingProvidersCount_InvokesQueryHandler(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesCountController sut,
        CourseTrainingProviderCountModel courseModel,
        GetCourseTrainingProvidersCountQuery courseQuery
    )
    {
        mediatorMock.Setup(m =>
            m.Send(
                It.Is<GetCourseTrainingProvidersCountQuery>(q =>
                    q.LarsCodes == courseQuery.LarsCodes &&
                    q.Distance == courseQuery.Distance &&
                    q.Latitude == courseQuery.Latitude &&
                    q.Longitude == courseQuery.Longitude
                ),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            new ValidatedResponse<GetCourseTrainingProvidersCountQueryResult>(
                new GetCourseTrainingProvidersCountQueryResult(
                    new List<CourseTrainingProviderCountModel>()
                    {
                        courseModel
                    }
                )
            )
        );

        var result = await sut.GetTrainingProvidersCount(courseQuery, CancellationToken.None);

        mediatorMock.Verify(m =>
            m.Send(It.Is<GetCourseTrainingProvidersCountQuery>(q =>
                q.LarsCodes == courseQuery.LarsCodes &&
                q.Longitude == courseQuery.Longitude &&
                q.Latitude == courseQuery.Latitude &&
                q.Distance == courseQuery.Distance
            ),
            It.IsAny<CancellationToken>()
        ));

        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var response = okResult.Value as GetCourseTrainingProvidersCountQueryResult;

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Courses.Count, Is.EqualTo(1));

            var model = response.Courses[0];
            Assert.That(model.LarsCode, Is.EqualTo(courseModel.LarsCode));
            Assert.That(model.ProvidersCount, Is.EqualTo(courseModel.ProvidersCount));
            Assert.That(model.TotalProvidersCount, Is.EqualTo(courseModel.TotalProvidersCount));
        });
    }
}
