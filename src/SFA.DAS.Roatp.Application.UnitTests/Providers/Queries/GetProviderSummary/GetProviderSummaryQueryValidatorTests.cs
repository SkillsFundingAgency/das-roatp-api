using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviderSummary;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviderSummary;

[TestFixture]
public class GetProviderSummaryQueryValidatorTests
{
    [Test]
    public async Task ValidateProviderSummaryQuery_UkprnOutsideRange_ErrorMessageReturned()
    {
        var providersReadRepository = new Mock<IProvidersReadRepository>();
        providersReadRepository.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

        var query = new GetProviderSummaryQuery(-1);
        var sut = new GetProviderSummaryQueryValidator(providersReadRepository.Object);

        var result = await sut.TestValidateAsync(query);

        result
            .ShouldHaveValidationErrorFor(q => q.Ukprn)
            .WithErrorMessage(UkprnValidator.InvalidUkprnErrorMessage);
    }

    [Test]
    public async Task ValidateProviderSummaryQuery_ProviderDoesNotExist_ErrorMessageReturned()
    {
        var providersReadRepository = new Mock<IProvidersReadRepository>();
        providersReadRepository.Setup(x => x.GetByUkprn(It.IsAny<int>())).ReturnsAsync((Provider)null);

        var query = new GetProviderSummaryQuery(10000001);
        var sut = new GetProviderSummaryQueryValidator(providersReadRepository.Object);

        var result = await sut.TestValidateAsync(query);

        result
            .ShouldHaveValidationErrorFor(q => q.Ukprn)
            .WithErrorMessage(UkprnValidator.ProviderNotFoundErrorMessage);
    }
}
