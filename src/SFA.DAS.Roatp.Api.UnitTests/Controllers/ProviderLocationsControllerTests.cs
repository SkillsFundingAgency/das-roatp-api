using System;
using System.Collections.Generic;
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
using SFA.DAS.Roatp.Application.Mediatr.Responses;
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
            List<ProviderLocationModel> locations)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<List<ProviderLocationModel>>(locations));
            var result = await sut.GetLocations(ukprn);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(locations);
        }

        [Test, MoqAutoData]
        public async Task GetLocation_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationsController sut,
            int ukprn,
            Guid id,
            ProviderLocationModel handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<ProviderLocationModel>(handlerResult));
            var result = await sut.GetLocation(ukprn, id);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}