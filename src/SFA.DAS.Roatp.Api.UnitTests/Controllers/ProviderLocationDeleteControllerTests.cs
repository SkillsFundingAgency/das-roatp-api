using System;
using System.Net;
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
        [Frozen] Mock<IMediator> _mediatorMock,
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn, Guid navigationId, string userId, string userDisplayName)
    {
        _mediatorMock.Setup(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        var result = await sut.DeleteProviderLocation(ukprn, navigationId, userId, userDisplayName);

        _mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderLocationCommand>(c => c.Ukprn == ukprn && c.Id == navigationId && c.UserId == userId && c.UserDisplayName == userDisplayName), It.IsAny<CancellationToken>()));
        var statusCodeResult = (NoContentResult)result;

        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }
}