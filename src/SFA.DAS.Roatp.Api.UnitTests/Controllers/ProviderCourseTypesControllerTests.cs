using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers
{
    [TestFixture]
    public class ProviderCourseTypesControllerTests
    {
        [Test, MoqAutoData]
        public async Task GetCourseTypes_CallsMediator(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] ProviderCourseTypesController sut,
            int ukprn,
            List<ProviderCourseTypeModel> handlerResult)
        {
            mediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<List<ProviderCourseTypeModel>>(handlerResult));
            var result = await sut.GetProviderCourseTypes(ukprn);
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
        }
    }
}
