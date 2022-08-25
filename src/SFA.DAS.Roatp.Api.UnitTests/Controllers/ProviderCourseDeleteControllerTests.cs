using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourse.Commands.DeleteProviderCourse;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseDeleteControllerTests
    {
        [Test, MoqAutoData]
        public async Task DeleteProviderCourse_CallsHandler(
            [Frozen] Mock<IMediator> _mediatorMock,
            [Greedy] ProviderCourseDeleteController sut,
            int ukprn, int larsCode, string userId)
        {
            var result =  await sut.DeleteProviderCourse(ukprn, larsCode, userId);

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteProviderCourseCommand>(c => c.Ukprn == ukprn && c.LarsCode == larsCode && c.UserId==userId), It.IsAny<CancellationToken>()));
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteUnmatchedRegionalProviderLocationsCommand>(c => c.Ukprn == ukprn && c.UserId==userId), It.IsAny<CancellationToken>()));

            var statusCodeResult = (NoContentResult)result;

            statusCodeResult.Should().NotBeNull();
        }
    }
}
