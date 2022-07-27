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
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.Application.Locations.Queries.GetProviderLocations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationsControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetLocations_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationsController sut,
            int ukprn,
            GetProviderLocationsQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetLocations(ukprn);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.Locations);
        }

        [Test, MoqAutoData]
        public async Task GetLocation_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationsController sut,
            int ukprn,
            Guid id,
            GetProviderLocationDetailsQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetLocation(ukprn, id);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult.Location);
        }
    }
}