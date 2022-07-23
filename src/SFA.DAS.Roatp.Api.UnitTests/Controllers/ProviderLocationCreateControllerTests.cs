using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
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
            int ukprn,
            ProviderLocationCreateModel model)
        {
            var response = await sut.CreateLocation(ukprn, model);

            mediatorMock.Verify(m => m.Send(It.Is<CreateProviderLocationCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>()));
            var result = (StatusCodeResult)response;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }
    }
}
