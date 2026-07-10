using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common.Models;
using SFA.DAS.Roatp.Application.Providers.Queries.GetAllowedProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvidersNotAllowed;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.CourseControllerTests;

public class CourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task WhenGettingProvidersInAllowedListForRestrictedCourse_AndLarsCodeIsInvalid_ThenReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        RestrictedCourseDetailsModel expected,
        GetAllowedProvidersQuery query)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetAllowedProvidersQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await sut.GetAllowedProvidersByCourse(query.LarsCode);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingProvidersInAllowedListForRestrictedCourse_AndLarsCodeIsInvalid_ThenReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        GetAllowedProvidersQuery query)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetAllowedProvidersQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RestrictedCourseDetailsModel)null);

        // Act
        var result = await sut.GetAllowedProvidersByCourse(query.LarsCode);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test, MoqAutoData]
    public async Task WhenGettingProvidersNotInAllowedListForRestrictedCourse_AndLarsCodeIsValid_ThenReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        RestrictedCourseDetailsModel expected,
        GetProvidersNotAllowedQuery query)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetProvidersNotAllowedQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await sut.GetProvidersNotAllowedByCourse(query.LarsCode);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expected);
    }

    [Test, MoqAutoData]
    public async Task WhenGettingProvidersNotInAllowedListForRestrictedCourse_AndLarsCodeIsValid_ThenReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseController sut,
        GetProvidersNotAllowedQuery query)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.Is<GetProvidersNotAllowedQuery>(q => q.LarsCode == query.LarsCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RestrictedCourseDetailsModel)null);

        // Act
        var result = await sut.GetProvidersNotAllowedByCourse(query.LarsCode);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
