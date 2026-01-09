using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Infrastructure;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.StandardsControllerTests;

[TestFixture]
public class StandardsControllerGetStandardForLarsCodeTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task GetStandardForLarsCode_StandardExists_ReturnsOk(
         GetStandardForLarsCodeQueryResult result,
         string larsCode,
         Standard standard,
         [Frozen] Mock<IMediator> mediatorMock,
         [Frozen] Mock<ILogger<StandardsController>> loggerMock,
         [Greedy] StandardsController sut)
    {
        standard.LarsCode = larsCode;

        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStandardForLarsCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetStandardForLarsCodeQueryResult>((GetStandardForLarsCodeQueryResult)standard));

        var actionResult = await sut.GetStandardForLarsCode(larsCode);

        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        var returned = okResult.Value as GetStandardForLarsCodeQueryResult;
        returned.Should().NotBeNull();

        returned.Should().BeEquivalentTo((GetStandardForLarsCodeQueryResult)standard);

        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Inner API: Request received to get standard for larsCode") && v.ToString().Contains(larsCode)),
                It.IsAny<System.Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task GetStandardForLarsCode_WhenResultIsNullAndValid_ReturnsNotFound(
        string larsCode,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ILogger<StandardsController>> loggerMock,
        [Greedy] StandardsController sut)
    {
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStandardForLarsCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetStandardForLarsCodeQueryResult>((GetStandardForLarsCodeQueryResult)null));

        var actionResult = await sut.GetStandardForLarsCode(larsCode);

        actionResult.Should().BeOfType<NotFoundResult>();

        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Inner API: Request received to get standard for larsCode") && v.ToString().Contains(larsCode)),
                It.IsAny<System.Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task GetStandardForLarsCode_WhenResponseIsInvalid_ReturnsBadRequest(
        string larsCode,
        List<ValidationFailure> validationFailures,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ILogger<StandardsController>> loggerMock,
        [Greedy] StandardsController sut)
    {
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStandardForLarsCodeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<GetStandardForLarsCodeQueryResult>(validationFailures));

        var actionResult = await sut.GetStandardForLarsCode(larsCode);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();

        var errors = badRequest.Value as List<ValidationError>;
        errors.Should().NotBeNull();
        errors.Should().ContainSingle(e => e.PropertyName == validationFailures[0].PropertyName && e.ErrorMessage == validationFailures[0].ErrorMessage);

        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Inner API: Request received to get standard for larsCode") && v.ToString().Contains(larsCode)),
                It.IsAny<System.Exception>(),
                It.IsAny<Func<It.IsAnyType, System.Exception, string>>()),
            Times.Once);
    }
}
