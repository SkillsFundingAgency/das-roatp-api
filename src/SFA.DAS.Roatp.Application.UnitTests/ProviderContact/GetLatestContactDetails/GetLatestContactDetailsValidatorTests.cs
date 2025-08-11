using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Common;
using SFA.DAS.Roatp.Application.ProviderContact.Queries.GetProviderContact;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderContact.GetLatestContactDetails;
public class GetLatestProviderContactValidatorTests
{
    [Test]
    public async Task ValidateUkprn_InValid_ReturnsError()
    {
        var ukprn = 10000000;
        var query = new GetLatestProviderContactQuery(ukprn);

        var sut = new GetLatestProviderContactQueryValidator(Mock.Of<IProvidersReadRepository>());

        var result = await sut.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.InvalidUkprnErrorMessage);
    }
}
