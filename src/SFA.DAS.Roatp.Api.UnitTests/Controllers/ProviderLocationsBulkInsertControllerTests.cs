using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkInsert;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationsBulkInsertControllerTests
    {
        [Test, MoqAutoData]
        public async Task ProviderLocationsBulkInsert_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderLocationsBulkInsertController sut,
            int ukprn, ProviderLocationsInsertModel providerLocationsInsertModel)
        {
            await sut.BulkInsertProviderLocations(ukprn, providerLocationsInsertModel);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkInsertProviderLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == providerLocationsInsertModel.LarsCode), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task ProviderLocationsBulkInsert_MoreThanZeroResults_ReturnsNoContentResponse(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderLocationsBulkInsertController sut,
            int ukprn, ProviderLocationsInsertModel providerLocationsInsertModel)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<BulkInsertProviderLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == providerLocationsInsertModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await sut.BulkInsertProviderLocations(ukprn, providerLocationsInsertModel);

            _mediatorMock.Verify(m => m.Send(It.Is<BulkInsertProviderLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == providerLocationsInsertModel.LarsCode), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }
    }
}
