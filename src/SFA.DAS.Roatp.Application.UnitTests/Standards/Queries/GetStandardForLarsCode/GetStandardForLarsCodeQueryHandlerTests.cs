using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Standards.Queries.GetStandardForLarsCode;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.Queries.GetStandardForLarsCode;

[TestFixture]
public class GetStandardForLarsCodeQueryHandlerTests
{
    [Test, RecursiveMoqAutoData()]
    public async Task Handle_WhenStandardExistsForLarsCode_ReturnsValidatedResponse(
        string larsCode,
        [Frozen] Mock<IStandardsReadRepository> repositoryMock,
        [Frozen] Mock<ILogger<GetStandardForLarsCodeQueryHandler>> loggerMock,
        GetStandardForLarsCodeQueryHandler sut)
    {
        var expectedStandard = new Standard { LarsCode = larsCode };
        repositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == larsCode)))
            .ReturnsAsync(expectedStandard);

        var query = new GetStandardForLarsCodeQuery(larsCode);

        var response = await sut.Handle(query, CancellationToken.None);

        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.Should().BeEquivalentTo((GetStandardForLarsCodeQueryResult)expectedStandard);

        repositoryMock.Verify(r => r.GetStandard(larsCode), Times.Once);

    }

    [Test, RecursiveMoqAutoData()]
    public async Task Handle_WhenStandardDoesNotExistForLarsCode_ReturnsValidatedResponseWithNullStandard(
        string larsCode,
        [Frozen] Mock<IStandardsReadRepository> repositoryMock,
        [Frozen] Mock<ILogger<GetStandardForLarsCodeQueryHandler>> loggerMock,
        GetStandardForLarsCodeQueryHandler sut)
    {
        repositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == larsCode)))
            .ReturnsAsync((Standard)null);

        var query = new GetStandardForLarsCodeQuery(larsCode);

        var response = await sut.Handle(query, CancellationToken.None);

        response.Should().NotBeNull();
        response.Result.Should().BeNull();

        repositoryMock.Verify(r => r.GetStandard(larsCode), Times.Once);
    }

    [Test, RecursiveMoqAutoData()]
    public async Task Handle_WhenStandardExists_VerifyLogger(
        string larsCode,
        [Frozen] Mock<IStandardsReadRepository> repositoryMock,
        [Frozen] Mock<ILogger<GetStandardForLarsCodeQueryHandler>> loggerMock,
        GetStandardForLarsCodeQueryHandler sut)
    {
        var expectedStandard = new Standard { LarsCode = larsCode };
        repositoryMock.Setup(r => r.GetStandard(It.Is<string>(s => s == larsCode)))
            .ReturnsAsync(expectedStandard);

        var query = new GetStandardForLarsCodeQuery(larsCode);

        var response = await sut.Handle(query, CancellationToken.None);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Returning standard for larsCode") && v.ToString().Contains(larsCode)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}