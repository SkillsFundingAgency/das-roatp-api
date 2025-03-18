using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Shortlists.Commands.DeleteExpiredShortlists;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ShortlistsControllerTests;

public class ShortlistsControllerDeleteExpiredShortlistsTests
{
    [Test]
    public async Task DeleteExpiredShortlists_ReturnsAccepted()
    {
        Mock<IMediator> mediatorMock = new();
        ShortlistsController sut = new(mediatorMock.Object);
        var response = await sut.DeleteExpiredShortlists(CancellationToken.None);
        response.As<AcceptedResult>().Should().NotBeNull();
        mediatorMock.Verify(mediatorMock => mediatorMock.Send(It.IsAny<DeleteExpiredShortlistsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
