using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Commands.DeleteLocation;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;


[TestFixture]
public class ProviderLocationDeleteControllerTests
{
    [Test, MoqAutoData]
    public async Task DeleteProviderLocation_CallsHandler(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn, Guid navigationId, string userId, string userDisplayName)
    {
        mediatorMock.Setup(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<bool>(true));

        var result = await sut.DeleteProviderLocation(ukprn, navigationId, userId, userDisplayName);

        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId && c.UserId == userId && c.UserDisplayName == userDisplayName), It.IsAny<CancellationToken>()));

        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
    }

    [Test, MoqAutoData]
    public async Task DeleteProviderLocation_CallsHandler_ReturnsNotFoundResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn, Guid navigationId, string userId, string userDisplayName)
    {
        mediatorMock.Setup(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<bool>(false));

        var result = await sut.DeleteProviderLocation(ukprn, navigationId, userId, userDisplayName);

        mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId && c.UserId == userId && c.UserDisplayName == userDisplayName), It.IsAny<CancellationToken>()));

        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
    }
}