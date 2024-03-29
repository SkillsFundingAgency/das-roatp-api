﻿using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationsBulkDeleteControllerTests
    {
        [Test, MoqAutoData]
        public async Task ProviderLocationsBulkDelete_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderLocationsBulkDeleteController sut,
            int ukprn,  string userId, string userDisplayName)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<int>(0));
            await sut.BulkDeleteProviderLocations(ukprn, userId, userDisplayName);
            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task ProviderLocationsBulkDelete_ZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderLocationsBulkDeleteController sut,
            int ukprn,  string userId, string userDisplayName)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync( new ValidatedResponse<int>(0));
            var result = await sut.BulkDeleteProviderLocations(ukprn,  userId, userDisplayName);
            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn ), It.IsAny<CancellationToken>()));
            var statusCodeResult = (NoContentResult)result;
            statusCodeResult.Should().NotBeNull();
        }
        
        [Test, MoqAutoData]
        public async Task ProviderLocationsBulkDelete_MoreThanZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderLocationsBulkDeleteController sut,
            int ukprn,  string userId, string userDisplayName)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<int>(1));
            var result = await sut.BulkDeleteProviderLocations(ukprn,  userId, userDisplayName);
            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderLocationsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>()));
            var statusCodeResult = (NoContentResult)result;
            statusCodeResult.Should().NotBeNull();
        }
    }
}
