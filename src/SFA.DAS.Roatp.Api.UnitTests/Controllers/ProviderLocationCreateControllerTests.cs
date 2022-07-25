using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationCreateControllerTests
    {
        [Test, MoqAutoData]
        public async Task CreateLocaiton_InvokesCommand(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderLocationCreateController sut,
            [Frozen] int ukprn,
            CreateProviderLocationCommand command)
        {
            var response = await sut.CreateLocation(ukprn, command);

            mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()));
            var result = (StatusCodeResult)response;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Test, MoqAutoData]
        public async Task CreateLocaiton_UkprnMismatch_ReturnsBadRequestResult(
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
