using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Locations.Commands.UpdateProviderLocationDetails;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;
using IMediator = MediatR.IMediator;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderLocationEditControllerTests
    {
        [Test, AutoData]
        public async Task Save_InvokesCommand(int ukprn, Guid id, ProviderLocationEditModel model)
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.Is<UpdateProviderLocationDetailsCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<bool>(true));

            var sut = new ProviderLocationEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderLocationEditController>>());

            var result = await sut.Save(ukprn, id, model);

            (result as NoContentResult).Should().NotBeNull();

            mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderLocationDetailsCommand>(c => c.Ukprn == ukprn && c.Id == id), It.IsAny<CancellationToken>()));
        }
    }
}
