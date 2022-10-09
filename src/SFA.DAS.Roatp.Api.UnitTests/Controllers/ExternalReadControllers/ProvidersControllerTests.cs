using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers.ExternalReadControllers;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ExternalReadControllers
{
    public class ProvidersControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetProviders_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProvidersController sut
            )
        {
            GetProvidersQueryResult handlerResult = new GetProvidersQueryResult();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProvidersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetProviders();

            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }

        [Test, MoqAutoData]
        public async Task GetProvider_CallsMediator(
           [Frozen] Mock<IMediator> mediatorMock,
           [Greedy] ProvidersController sut,
           int ukprn,
           GetProviderSummaryQueryResult handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderSummaryQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResult);

            var result = await sut.GetProvider(ukprn);

            (result as OkObjectResult).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
