using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;
public class ProviderContactControllerTests
{
    [Test, MoqAutoData]
    public async Task GetLatestProviderContact_CallsMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn,
        GetLatestProviderContactQueryResult handlerResult)
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetLatestProviderContactQueryResult>(handlerResult));
        var result = await sut.GetLatestProviderContact(ukprn);
        ((OkObjectResult)result).Value.Should().BeEquivalentTo(handlerResult);
    }

    [Test, MoqAutoData]
    public async Task GetLatestProviderContact_CallsMediator_InvalidUkprn_Returns_BadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn,
        GetLatestProviderContactQueryResult handlerResult)
    {
        List<ValidationFailure> errors = new List<ValidationFailure>
        {
            new() { ErrorMessage = UkprnValidator.InvalidUkprnErrorMessage }
        };

        ValidatedResponse<GetLatestProviderContactQueryResult> validatedResponse = new(errors);

        mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(validatedResponse);
        var result = await sut.GetLatestProviderContact(ukprn);
        result.As<BadRequestObjectResult>().Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task GetLatestProviderContact_CallsMediator_UkprnHasNoRecords_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn)

    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetLatestProviderContactQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<GetLatestProviderContactQueryResult>((GetLatestProviderContactQueryResult)null));
        var result = await sut.GetLatestProviderContact(ukprn);
        result.As<NotFoundResult>().Should().NotBeNull();
    }
}
