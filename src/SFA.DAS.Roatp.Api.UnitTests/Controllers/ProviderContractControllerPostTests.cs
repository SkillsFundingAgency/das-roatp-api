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
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderContact.Commands.CreateProviderContact;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers;

[TestFixture]
public class ProviderContractControllerPostTests
{
    [Test, MoqAutoData]
    public async Task PostProviderContact_CallsMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn,
        ProviderContactAddModel providerContactAddModel,
        long providerContactId)
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<CreateProviderContactCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse<long>(providerContactId));
        var result = await sut.PostProviderContact(ukprn, providerContactAddModel);
        var createdResult = result as CreatedResult;
        (createdResult!).Value.Should().BeEquivalentTo(providerContactId);
        createdResult.Location.Should().Be($"providers/{ukprn}/contact");
    }

    [Test, MoqAutoData]
    public async Task PostProviderContact_CallsMediator_Error_Returns_BadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderContactController sut,
        int ukprn,
        ProviderContactAddModel providerContactAddModel)
    {
        List<ValidationFailure> errors = new List<ValidationFailure>
        {
            new() { ErrorMessage = UkprnValidator.InvalidUkprnErrorMessage }
        };

        ValidatedResponse<long> validatedResponse = new(errors);

        mediatorMock.Setup(m => m.Send(It.IsAny<CreateProviderContactCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(validatedResponse);
        var result = await sut.PostProviderContact(ukprn, providerContactAddModel);
        result.As<BadRequestObjectResult>().Should().NotBeNull();
    }
}
