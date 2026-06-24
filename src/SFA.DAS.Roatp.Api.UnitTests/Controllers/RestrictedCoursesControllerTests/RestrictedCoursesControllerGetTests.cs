using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.RestrictedCourses.Queries.GetAllRestrictedCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.RestrictedCoursesControllerTests;

public class RestrictedCoursesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task WhenGetRestrictedCoursesIsInvoked_ThenReturnsOkResult(
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] RestrictedCoursesController sut,
        GetAllRestrictedCoursesQueryResult expected)
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRestrictedCoursesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await sut.GetRestrictedCourses(true);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expected);
    }
}
