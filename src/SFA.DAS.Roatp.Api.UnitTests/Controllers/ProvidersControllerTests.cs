using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProvider;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    public class ProvidersControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetProvider_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut,
            int ukprn,
            GetProviderQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetProvider(ukprn);

            (result.Result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
