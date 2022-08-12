using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddNationalLocation;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseLocationsEditControllerTests
    {
        [Test, RecursiveMoqAutoData]
        public async Task AddNationalLocationToProviderCourseLocations_CallsHandler(int ukprn, int larsCode, AddNationalLocationToProviderCourseLocationsModel model, ProviderCourseLocation providerCourseLocation)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<AddNationalLocationToProviderCourseLocationsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseLocation);
            var sut = new ProviderCourseLocationsEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationsEditController>>());
            
            var response = await sut.AddNationalLocationToProviderCourseLocations(ukprn, larsCode, model);

            var result = (CreatedAtRouteResult)response;

            result.Should().NotBeNull();

            result.Value.Should().Be(providerCourseLocation.Id);
            result.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);

            mediatorMock.Verify(m => m.Send(It.Is<AddNationalLocationToProviderCourseLocationsCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.UserId == model.UserId), It.IsAny<CancellationToken>()));
        }

        [Test, RecursiveMoqAutoData]
        public async Task CreateProviderCourseLocation_CallsHandler(int ukprn, int larsCode, AddProviderCourseLocationModel model, ProviderCourseLocation providerCourseLocation)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<AddProviderCourseLocationCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseLocation.Id);
            var sut = new ProviderCourseLocationsEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderCourseLocationsEditController>>());

            var response = await sut.CreateProviderCourseLocation(ukprn, larsCode, model);

            var result = (CreatedAtRouteResult)response;

            result.Should().NotBeNull();

            result.Value.Should().Be(providerCourseLocation.Id);
            result.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);

            mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseLocationCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.UserId == model.UserId), It.IsAny<CancellationToken>()));
        }

    }
}
