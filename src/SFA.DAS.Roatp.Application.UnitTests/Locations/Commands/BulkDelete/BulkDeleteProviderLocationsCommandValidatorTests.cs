using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderLocationsCommandValidatorTests
    {
        private readonly string _userId = "userid";
        private readonly string _userDisplayName = "userDisplayName";
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderLocationsCommand(10012002,  _userId, _userDisplayName);

            var sut = new BulkDeleteProviderLocationsCommandValidator(Mock.Of<IProvidersReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkDeleteProviderLocationsCommand(10012002, userId, _userDisplayName);
            var sut = new BulkDeleteProviderLocationsCommandValidator(Mock.Of<IProvidersReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
