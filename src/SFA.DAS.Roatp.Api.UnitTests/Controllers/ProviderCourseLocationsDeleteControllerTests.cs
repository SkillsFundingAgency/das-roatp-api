using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseLocationsDeleteControllerTests
    {
        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsDeleteController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options, string userId)
        {
            await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options, userId);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_ZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsDeleteController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options, string userId)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>())).ReturnsAsync(0);

            var result = await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options, userId);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_MoreThanZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsDeleteController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options, string userId)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options, userId);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task DeleteProviderCourseLocation_CallsHandler(
           [Frozen] Mock<IMediator> _mediatorMock,
           [Greedy] ProviderCourseLocationsDeleteController sut,
           int ukprn, int larsCode, Guid id, string userId, string userDisplayName)
        {
            await sut.DeleteProviderCourseLocation(ukprn, larsCode, id, userId, userDisplayName);

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.LocationId == id), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task DeleteProviderCourseLocation_ZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseLocationsDeleteController sut,
            int ukprn, int larsCode, Guid id, string userId, string userDisplayName)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.LocationId == id), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var result = await sut.DeleteProviderCourseLocation(ukprn, larsCode, id, userId, userDisplayName);

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseLocationCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.LocationId == id), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }
    }
}
