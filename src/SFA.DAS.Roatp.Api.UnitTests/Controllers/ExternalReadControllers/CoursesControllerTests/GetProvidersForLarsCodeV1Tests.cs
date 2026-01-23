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
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersForLarsCodeV1Tests
    {
        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV1_InvokesQueryHandler(
            GetProvidersFromLarsCodeRequest request,
            GetProvidersForLarsCodeQueryResultV2 queryResultV2,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            const int larsCodeInt = 1;
            queryResultV2.LarsCode = larsCodeInt.ToString();

            mediatorMock.Setup(m => m.Send(
                    It.Is<GetProvidersForLarsCodeQueryV2>(q =>
                        q.LarsCode == larsCodeInt.ToString()
                        && q.Latitude == request.Latitude
                        && q.Longitude == request.Longitude
                        && q.OrderBy == request.OrderBy
                        && q.Distance == request.Distance
                        && q.Page == request.Page
                        && q.PageSize == request.PageSize
                        && q.Location == request.Location
                        && q.UserId == request.UserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidatedResponse<GetProvidersForLarsCodeQueryResultV2>(queryResultV2));

            var result = await sut.GetProvidersForLarsCode(larsCodeInt, request);

            var ok = result.As<OkObjectResult>();
            ok.Should().NotBeNull();

            var actualResult = ok.Value.As<GetProvidersForLarsCodeQueryResult>();
            actualResult.Should().NotBeNull();

            using (new AssertionScope())
            {
                actualResult.Page.Should().Be(queryResultV2.Page);
                actualResult.PageSize.Should().Be(queryResultV2.PageSize);
                actualResult.TotalPages.Should().Be(queryResultV2.TotalPages);
                actualResult.TotalCount.Should().Be(queryResultV2.TotalCount);
                actualResult.LarsCode.Should().Be(larsCodeInt);
                actualResult.StandardName.Should().Be(queryResultV2.StandardName);
                actualResult.QarPeriod.Should().Be(queryResultV2.QarPeriod);
                actualResult.ReviewPeriod.Should().Be(queryResultV2.ReviewPeriod);
            }

            mediatorMock.Verify(m => m.Send(
                It.Is<GetProvidersForLarsCodeQueryV2>(q =>
                    q.LarsCode == larsCodeInt.ToString()
                    && q.Latitude == request.Latitude
                    && q.Longitude == request.Longitude
                    && q.OrderBy == request.OrderBy
                    && q.Distance == request.Distance
                    && q.Page == request.Page
                    && q.PageSize == request.PageSize
                    && q.Location == request.Location
                    && q.UserId == request.UserId),
                It.IsAny<CancellationToken>()));
        }
    }
}
