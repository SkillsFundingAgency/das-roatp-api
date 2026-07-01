using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.CourseControllerTests;

public class CourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task WhenGetAllowedProvidersByCourseIsInvoked_ThenReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        GetAllowedProvidersQueryResult expected,
        GetAllowedProvidersQuery query)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetAllowedProvidersQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetAllowedProvidersQueryResult>(expected));

        // Act
        var result = await sut.GetAllowedProvidersByCourse(query.LarsCode);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public async Task WhenGetAllowedProvidersByCourseIsInvokedAndResponseIsInvalid_ThenReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        GetAllowedProvidersQuery query,
        List<ValidationFailure> validationFailures)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetAllowedProvidersQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetAllowedProvidersQueryResult>(validationFailures));

        // Act
        var result = await sut.GetAllowedProvidersByCourse(query.LarsCode);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
