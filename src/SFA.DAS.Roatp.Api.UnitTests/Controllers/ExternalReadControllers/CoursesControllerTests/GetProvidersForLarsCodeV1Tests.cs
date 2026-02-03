using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Api.Models.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersForLarsCode;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers.CoursesControllerTests
{
    [TestFixture]
    public class GetProvidersForLarsCodeV1Tests
    {
        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV1_InvokesQueryHandler(
            GetProvidersForLarsCodeRequest request,
            GetProvidersForLarsCodeQueryResult queryResult,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            const int larsCodeInt = 1;
            queryResult.LarsCode = larsCodeInt.ToString();

            mediatorMock.Setup(m => m.Send(
                    It.Is<GetProvidersForLarsCodeQuery>(q =>
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
                .ReturnsAsync(new ValidatedResponse<GetProvidersForLarsCodeQueryResult>(queryResult));

            var result = await sut.GetProvidersForLarsCode(larsCodeInt, request);

            var ok = result.As<OkObjectResult>();
            ok.Should().NotBeNull();

            var actualResult = ok.Value.As<GetProvidersForLarsCodeResultModel>();
            actualResult.Should().NotBeNull();

            using (new AssertionScope())
            {
                actualResult.Page.Should().Be(queryResult.Page);
                actualResult.PageSize.Should().Be(queryResult.PageSize);
                actualResult.TotalPages.Should().Be(queryResult.TotalPages);
                actualResult.TotalCount.Should().Be(queryResult.TotalCount);
                actualResult.LarsCode.Should().Be(larsCodeInt);
                actualResult.StandardName.Should().Be(queryResult.StandardName);
                actualResult.QarPeriod.Should().Be(queryResult.QarPeriod);
                actualResult.ReviewPeriod.Should().Be(queryResult.ReviewPeriod);
            }

            mediatorMock.Verify(m => m.Send(
                It.Is<GetProvidersForLarsCodeQuery>(q =>
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

        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV1_WhenResultNullAndValid_ReturnsNotFound(
            GetProvidersForLarsCodeRequest request,
            int larsCodeInt,
            GetProvidersForLarsCodeQueryResult queryResult,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var response = new ValidatedResponse<GetProvidersForLarsCodeQueryResult>(new List<ValidationFailure>());
            mediatorMock.Setup(m => m.Send(
                  It.Is<GetProvidersForLarsCodeQuery>(q =>
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
              .ReturnsAsync(response);

            var result = await sut.GetProvidersForLarsCode(larsCodeInt, request);

            result.Should().BeOfType<NotFoundResult>();

            mediatorMock.Verify(m => m.Send(
                It.Is<GetProvidersForLarsCodeQuery>(q => q.LarsCode == larsCodeInt.ToString()),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetProvidersForLarsCodeV1_WhenInvalid_ReturnsBadRequest(
            GetProvidersForLarsCodeRequest request,
            int larsCodeInt,
            GetProvidersForLarsCodeQueryResult queryResult,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] CoursesController sut)
        {
            var validationErrors = new List<ValidationFailure>
            {
             new ValidationFailure("Latitude", "Latitude is required"),
             new ValidationFailure("Longitude", "Longitude is required")
            };

            var response = new ValidatedResponse<GetProvidersForLarsCodeQueryResult>(validationErrors);

            mediatorMock.Setup(m => m.Send(
                  It.Is<GetProvidersForLarsCodeQuery>(q =>
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
              .ReturnsAsync(response);

            var result = await sut.GetProvidersForLarsCode(larsCodeInt, request);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = (BadRequestObjectResult)result;
            badRequest.Value.Should().NotBeNull();

            badRequest.Value.Should().BeAssignableTo<List<SFA.DAS.Roatp.Api.Infrastructure.ValidationError>>();
            var errors = (List<SFA.DAS.Roatp.Api.Infrastructure.ValidationError>)badRequest.Value;
            errors.Should().HaveCount(2);
            errors[0].PropertyName.Should().Be("Latitude");
            errors[0].ErrorMessage.Should().Be("Latitude is required");
            errors[1].PropertyName.Should().Be("Longitude");
            errors[1].ErrorMessage.Should().Be("Longitude is required");

            mediatorMock.Verify(m => m.Send(
                It.Is<GetProvidersForLarsCodeQuery>(q => q.LarsCode == larsCodeInt.ToString()),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
