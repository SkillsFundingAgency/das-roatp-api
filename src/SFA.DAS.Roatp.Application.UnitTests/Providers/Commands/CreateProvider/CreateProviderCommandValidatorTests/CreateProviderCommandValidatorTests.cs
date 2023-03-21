using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using SFA.DAS.Roatp.Application.Providers.Commands.CreateProvider;
using Moq;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Application.Common;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Commands.CreateProvider.CreateProviderCommandValidatorTests
{
    [TestFixture]
    public class CreateProviderCommandValidatorTests
    {
        private Mock<IProvidersReadRepository> _providersReadRepositoryMock;
      
        public const int UkprnAlreadyPresent = 10012002;
        public const int UkprnNotAlreadyPresent = 11111111;

        [TestCase(1111111, false, UkprnValidator.InvalidUkprnErrorMessage)]
        [TestCase(111111111, false, UkprnValidator.InvalidUkprnErrorMessage)]
        [TestCase(UkprnNotAlreadyPresent, true, null)]
        [TestCase(UkprnAlreadyPresent, false,CreateProviderCommandValidator.UkprnAlreadyPresent)]
        public async Task Ukprn_Validation(int ukprn, bool isValid, string errorMessage)
        {
            var command = new CreateProviderCommand { Ukprn = ukprn, LegalName = "legal name", UserDisplayName = "display name", UserId = "user id"};
            var sut = GetValidator();

            var result = await sut.TestValidateAsync(command);
            
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.Ukprn);
                result.Errors[0].ErrorMessage.Should().Be(errorMessage);
            }
        }

        [TestCase("legal name", true)]
        [TestCase("", false)]
        [TestCase(null, false)]

        public async Task LegalNamePresent_Validation(string legalName, bool isValid)
        {
            var command = new CreateProviderCommand { Ukprn = UkprnNotAlreadyPresent, LegalName = legalName, UserDisplayName = "display name", UserId = "user id" };
            var sut = GetValidator();

            var result = await sut.TestValidateAsync(command);
            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.LegalName);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.LegalName);
                result.Errors[0].ErrorMessage.Should().Be(CreateProviderCommandValidator.LegalNameRequired);
            }
        }

        protected CreateProviderCommandValidator GetValidator()
        {
            _providersReadRepositoryMock = new Mock<IProvidersReadRepository>();
            _providersReadRepositoryMock.Setup(p => p.GetByUkprn(UkprnAlreadyPresent)).ReturnsAsync(new Provider { Ukprn = UkprnAlreadyPresent });

            return new CreateProviderCommandValidator(_providersReadRepositoryMock.Object);
        }
    }
}
