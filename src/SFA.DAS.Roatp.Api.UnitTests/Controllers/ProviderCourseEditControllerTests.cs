using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.UpdateProviderCourse;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseEditControllerTests
    {
        [Test, AutoData]
        public async Task Save_InvokesCommand(int ukprn, int larsCode, ProviderCourseEditModel model)
        {
            var mediatorMock = new Mock<IMediator>();
            var sut = new ProviderCourseEditController(mediatorMock.Object, Mock.Of<ILogger<ProviderCourseEditController>>());

            var result = await sut.Save(ukprn, larsCode, model);

            (result as NoContentResult).Should().NotBeNull();

            mediatorMock.Verify(m => m.Send(It.Is<UpdateProviderCourseCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode), It.IsAny<CancellationToken>()));
        }
    }
}
