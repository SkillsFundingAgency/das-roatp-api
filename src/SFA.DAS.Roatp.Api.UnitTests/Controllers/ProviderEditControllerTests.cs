﻿using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Application.Providers.Commands.PatchProvider;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    public class ProviderEditControllerTests
    {
        [Test]
        public async Task PatchProvider_InvokesRequest()
        {
            var ukprn = 10000001;
            var request = new JsonPatchDocument<PatchProvider>();
            var userId = "userId";
            var userDisplayName = "userDisplayName";


            var mediatorMock = new Mock<IMediator>();
            var sut = new ProviderEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderEditController>>());

            var result = await sut.PatchProvider(ukprn, request, userId, userDisplayName);

            (result as NoContentResult).Should().NotBeNull();

            mediatorMock.Verify(m => m.Send(It.Is<PatchProviderCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task CreateProvider_InvokesRequest(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderEditController sut,
            int ukprn,  ProviderAddModel model, int providerId, string userId, string userDisplayName)
        {
            mediatorMock.Setup(m => m.Send(It.Is<CreateProviderCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<int>(providerId));

            var result = await sut.CreateProvider(ukprn, model, userId, userDisplayName);

            (result as CreatedResult).Should().NotBeNull();
            result.As<CreatedResult>().Location.Should().Be($"/providers/{ukprn}");
            mediatorMock.Verify(m => m.Send(It.Is<CreateProviderCommand>(c => c.Ukprn == ukprn), It.IsAny<CancellationToken>()));

        }
    }
}
