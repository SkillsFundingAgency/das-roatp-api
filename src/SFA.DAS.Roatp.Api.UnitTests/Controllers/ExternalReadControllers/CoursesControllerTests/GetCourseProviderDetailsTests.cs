using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests;

public sealed class GetCourseProviderDetailsTests
{
    [Test]
    [MoqAutoData]
    public async Task GetCourseProviderDetails_InvokesQueryHandler(
        GetCourseProviderDetailsQuery query,
        GetCourseProviderDetailsQueryResult queryResult,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CoursesController sut
    )
    {
        int larsCode = 1;
        int ukprn = 2;

        mediatorMock.Setup(m => m.Send(
            It.Is<GetCourseProviderDetailsQuery>(q => 
                q.LarsCode == larsCode &&
                q.Ukprn == ukprn && 
                q.Location == query.Location &&
                q.Longitude == query.Longitude &&
                q.ShortlistUserId == query.ShortlistUserId
            ), 
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new ValidatedResponse<GetCourseProviderDetailsQueryResult>(queryResult));

        await sut.GetCourseProviderDetails(larsCode, ukprn, query);

        Assert.Multiple(() =>
        {
            Assert.That(query.LarsCode, Is.EqualTo(larsCode));
            Assert.That(query.Ukprn, Is.EqualTo(ukprn));
        });

        mediatorMock.Verify(m => m.Send(It.Is<GetCourseProviderDetailsQuery>(
            q =>
                q.LarsCode == larsCode
                && q.Latitude == query.Latitude
                && q.Longitude == query.Longitude
                && q.Location == query.Location
                && q.ShortlistUserId == query.ShortlistUserId
        ), It.IsAny<CancellationToken>()));
    }
}
