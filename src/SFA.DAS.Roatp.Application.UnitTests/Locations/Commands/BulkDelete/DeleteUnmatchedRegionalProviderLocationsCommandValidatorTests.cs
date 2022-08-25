using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkDelete
{
    [TestFixture]
    public class DeleteUnmatchedRegionalProviderLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new DeleteUnmatchedRegionalProviderLocationsCommand(10012002, _userId);

            var sut = new DeleteUnmatchedRegionalProviderLocationsCommandValidator(Mock.Of<IProviderReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new DeleteUnmatchedRegionalProviderLocationsCommand(10012002, userId);
            var sut = new DeleteUnmatchedRegionalProviderLocationsCommandValidator(Mock.Of<IProviderReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
