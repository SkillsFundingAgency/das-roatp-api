using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersForLarsCodeV2Tests
    {
        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV2_InvokesQueryHandler(
            GetProvidersForLarsCodeRequest request,
            GetProvidersForLarsCodeQueryResult queryResult,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            const string larsCodeString = "1";
            queryResult.LarsCode = larsCodeString;

            mediatorMock.Setup(m => m.Send(
                It.Is<GetProvidersForLarsCodeQuery>(
                    q =>
                        q.LarsCode == larsCodeString
                        && q.Latitude == request.Latitude
                        && q.Longitude == request.Longitude
                        && q.OrderBy == request.OrderBy
                        && q.Distance == request.Distance
                        && q.Page == request.Page
                        && q.PageSize == request.PageSize
                        && q.Location == request.Location
                        && q.UserId == request.UserId
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidatedResponse<GetProvidersForLarsCodeQueryResult>(queryResult));

            var result = await sut.GetProvidersForLarsCode(larsCodeString, request);

            var actualResult = result.As<OkObjectResult>().Value.As<GetProvidersForLarsCodeQueryResult>();

            using (new AssertionScope())
            {
                actualResult.Page.Should().Be(queryResult.Page);
                actualResult.PageSize.Should().Be(queryResult.PageSize);
                actualResult.TotalPages.Should().Be(queryResult.TotalPages);
                actualResult.TotalCount.Should().Be(queryResult.TotalCount);
                actualResult.LarsCode.Should().Be(larsCodeString);
                actualResult.StandardName.Should().Be(queryResult.StandardName);
                actualResult.QarPeriod.Should().Be(queryResult.QarPeriod);
                actualResult.ReviewPeriod.Should().Be(queryResult.ReviewPeriod);
            }

            mediatorMock.Verify(m => m.Send(It.Is<GetProvidersForLarsCodeQuery>(
                q =>
                    q.LarsCode == larsCodeString
                    && q.Latitude == request.Latitude
                    && q.Longitude == request.Longitude
                    && q.OrderBy == request.OrderBy
                    && q.Distance == request.Distance
                    && q.Page == request.Page
                    && q.PageSize == request.PageSize
                    && q.Location == request.Location
                    && q.UserId == request.UserId
            ), It.IsAny<CancellationToken>()));
        }
    }
}
