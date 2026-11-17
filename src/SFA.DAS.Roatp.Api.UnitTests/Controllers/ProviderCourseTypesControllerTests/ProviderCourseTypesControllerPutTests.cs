using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Controllers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Application.Mediatr.Responses;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Commands.PutProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Controllers.ProviderCourseTypesControllerTests;

public class ProviderCourseTypesControllerPutTests
{
    [Test, MoqAutoData]
    public async Task WhenRequestIsValid_ThenReturnsNoContentResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCourseTypesController sut,
        AddCourseTypesModel request,
        int ukprn)
    {
        // Arrange
        request.CourseTypes = [CourseType.Apprenticeship];

        mediatorMock
            .Setup(x => x.Send(It.IsAny<PutProviderCourseTypesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(Unit.Value));

        // Act
        var result = await sut.PutCourseTypes(ukprn, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        mediatorMock.Verify(x => x.Send(
            It.Is<PutProviderCourseTypesCommand>(c =>
                c.Ukprn == ukprn &&
                c.CourseTypes == request.CourseTypes &&
                c.UserId == request.UserId &&
                c.UserDisplayName == request.UserDisplayName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenValidationFails_ThenReturnsBadRequest(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ProviderCourseTypesController sut,
        AddCourseTypesModel request,
        int ukprn)
    {
        // Arrange
        request.CourseTypes = [CourseType.Apprenticeship];

        var errors = new List<ValidationFailure>
        {
            new(nameof(PutProviderCourseTypesCommand.CourseTypes), PutProviderCourseTypesCommandValidator.CourseTypesNotProvided)
        };

        mediatorMock
            .Setup(x => x.Send(It.IsAny<PutProviderCourseTypesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidatedResponse<Unit>(errors));

        // Act
        var result = await sut.PutCourseTypes(ukprn, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
