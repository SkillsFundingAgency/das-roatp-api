using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.AcademicYears.Queries.GetLatest;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers;
public class AcademicYearsControllerTests
{
    [Test, MoqAutoData]
    public async Task GetLatestAcademicYears_CallsMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AcademicYearsController sut,
        GetAcademicYearsLatestQueryResult handlerResult
    )
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAcademicYearsLatestQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);
        var result = await sut.GetLatestAcademicYears();
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
    }
}
