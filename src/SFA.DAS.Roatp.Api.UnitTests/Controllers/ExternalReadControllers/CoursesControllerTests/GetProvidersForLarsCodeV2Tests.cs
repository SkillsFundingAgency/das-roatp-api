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
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersForLarsCodeV2Tests
    {
        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV2_InvokesQueryHandler(
            GetProvidersFromLarsCodeRequestV2 request,
            GetProvidersForLarsCodeQueryResultV2 queryResultV2,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            const string larsCodeString = "1";
            queryResultV2.LarsCode = larsCodeString;

            mediatorMock.Setup(m => m.Send(
                It.Is<GetProvidersForLarsCodeQueryV2>(
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
                .ReturnsAsync(new ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>(queryResultV2));

            var result = await sut.GetProvidersForLarsCode(larsCodeString, request);

            var actualResult = result.As<OkObjectResult>().Value.As<GetProvidersForLarsCodeQueryResultV2>();

            using (new AssertionScope())
            {
                actualResult.Page.Should().Be(queryResultV2.Page);
                actualResult.PageSize.Should().Be(queryResultV2.PageSize);
                actualResult.TotalPages.Should().Be(queryResultV2.TotalPages);
                actualResult.TotalCount.Should().Be(queryResultV2.TotalCount);
                actualResult.LarsCode.Should().Be(larsCodeString);
                actualResult.StandardName.Should().Be(queryResultV2.StandardName);
                actualResult.QarPeriod.Should().Be(queryResultV2.QarPeriod);
                actualResult.ReviewPeriod.Should().Be(queryResultV2.ReviewPeriod);
            }

            mediatorMock.Verify(m => m.Send(It.Is<GetProvidersForLarsCodeQueryV2>(
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
