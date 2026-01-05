using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

[TestFixture]
public class StandardsControllerTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task GetAllStandards_ReturnsListOfStandards(
        List<Standard> standards,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ILogger<StandardsController>> loggerMock,
        [Greedy] StandardsController sut)
    {
        mediatorMock
            .Setup(r => r.Send(It.IsAny<GetAllStandardsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAllStandardsQueryResult(standards));

        var response = await sut.GetAllStandards();

        var result = response.Result as OkObjectResult;
        result.Should().NotBeNull();
        var queryResult = result.Value as GetAllStandardsQueryResult;

        queryResult.Standards.Should().BeEquivalentTo(standards, options => options.ExcludingMissingMembers());
    }

    [Test, RecursiveMoqAutoData()]
    public async Task GetAllStandards_WithCourseType_MediatorReceivedCourseType(
        CourseType courseType,
        List<Standard> standards,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ILogger<StandardsController>> loggerMock,
        [Greedy] StandardsController sut)
    {
        mediatorMock
            .Setup(r => r.Send(It.Is<GetAllStandardsQuery>(q => q.CourseType == courseType), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAllStandardsQueryResult(standards));

        var response = await sut.GetAllStandards(courseType);

        mediatorMock.Verify(r => r.Send(It.Is<GetAllStandardsQuery>(q => q.CourseType == courseType), It.IsAny<CancellationToken>()), Times.Once);
    }
}