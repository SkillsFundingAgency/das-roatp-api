using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class DeleteProviderCourseLocationsControllerTests
    {
        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] DeleteProviderCourseLocationsController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options)
        {
            await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_ZeroResults_ReturnsNotFoundResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] DeleteProviderCourseLocationsController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>())).ReturnsAsync(0);

            var result = await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));

            var statusCodeResult = (StatusCodeResult)result;

            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(404);
        }

        [Test, MoqAutoData]
        public async Task BulkDeleteProviderCourseLocations_MoreThanZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] DeleteProviderCourseLocationsController sut,
            int ukprn, int larsCode, DeleteProviderCourseLocationOption options)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await sut.BulkDeleteProviderCourseLocations(ukprn, larsCode, options);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkDeleteProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.DeleteProviderCourseLocationOptions == options), It.IsAny<CancellationToken>()));

            var statusCodeResult = (StatusCodeResult)result;

            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(204);
        }
    }
}
