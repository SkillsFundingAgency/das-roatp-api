using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationCreateControllerTests
    {
        [Test, MoqAutoData]
        public async Task CreateLocation_InvokesCommand(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationCreateController sut,
            [Frozen] int ukprn,
            CreateProviderLocationCommand command,
            int providerLocationId)
        {
            mediatorMock.Setup(m => m.Send(It.Is<CreateProviderLocationCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<int>(providerLocationId));
            var response = await sut.CreateLocation(ukprn, command);
            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()));
            var result = (CreatedResult)response;
            result.Should().NotBeNull();
            result.Value.Should().Be(providerLocationId);
        }

        [Test, MoqAutoData]
        public async Task CreateLocation_UkprnMismatch_ReturnsBadRequestResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationCreateController sut,
            int ukprn,
            CreateProviderLocationCommand command)
        {
            var response = await sut.CreateLocation(ukprn, command);
        
            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Never);
            var result = (BadRequestObjectResult)response;
            result.Should().NotBeNull();
        }
    }
}
