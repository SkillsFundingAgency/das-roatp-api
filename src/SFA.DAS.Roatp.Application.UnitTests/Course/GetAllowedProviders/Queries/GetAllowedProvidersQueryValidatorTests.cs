using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Course.GetAllowedProviders.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Course.GetAllowedProviders.Queries;

public class GetAllowedProvidersQueryValidatorTests
{
    [Test, MoqAutoData]
    public async Task WhenLarsCodeDoesNotExist_ThenValidationShouldFail(
   [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
   [Greedy] GetAllowedProvidersQueryValidator sut)
    {
        // Arrange
        var query = new GetAllowedProvidersQuery("123456");

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync((Standard)null);

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LarsCode)
            .WithErrorMessage(LarsCodeValidator.NotFoundMessage);
    }

    [Test, MoqAutoData]
    public async Task WhenLarsCodeIsNotRestricted_ThenValidationShouldPass(
        [Frozen] Mock<IStandardsReadRepository> standardsReadRepository,
        [Greedy] GetAllowedProvidersQueryValidator sut)
    {
        // Arrange
        var query = new GetAllowedProvidersQuery("12345");

        standardsReadRepository
            .Setup(r => r.GetStandard(It.IsAny<string>()))
            .ReturnsAsync(new Standard { LarsCode = query.LarsCode });

        // Act
        var result = await sut.TestValidateAsync(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LarsCode);
    }
}
