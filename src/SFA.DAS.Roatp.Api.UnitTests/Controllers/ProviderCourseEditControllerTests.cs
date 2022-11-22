using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using SFA.DAS.Roatp.Domain.Models;
using AutoFixture.NUnit3;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.CreateProviderCourse;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.PatchProviderCourse;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseEditControllerTests
    {
        [Test]
        public async Task PatchProviderCourse_InvokesRequest()
        {
            var ukprn = 10000001;
            var  larsCode = 1;
            var request = new JsonPatchDocument<PatchProviderCourse>();
            var userId = "userId";
            var userDisplayName = "userDisplayName";


            var mediatorMock = new Mock<IMediator>();
            var sut = new ProviderCourseEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderCourseEditController>>());

            var result = await sut.PatchProviderCourse(ukprn, larsCode, request, userId, userDisplayName);

            (result as NoContentResult).Should().NotBeNull();

            mediatorMock.Verify(m => m.Send(It.Is<PatchProviderCourseCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode), It.IsAny<CancellationToken>()));
        }

        [Test, MoqAutoData]
        public async Task CreateProviderCourse_InvokesRequest(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCourseEditController sut,
            int ukprn, int larsCode, ProviderCourseAddModel model, int providerCourseId, string userId, string userDisplayName)
        {
            mediatorMock.Setup(m => m.Send(It.Is<CreateProviderCourseCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseId);

            var result = await sut.CreateProviderCourse(ukprn, larsCode, model, userId, userDisplayName);

            result.As<CreatedResult>().Location.Should().Be($"/providers/{ukprn}/courses");
        }
    }
}
