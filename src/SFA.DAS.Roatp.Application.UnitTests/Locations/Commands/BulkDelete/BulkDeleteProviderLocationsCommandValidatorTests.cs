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
        [Test]
        public async Task ValidateUkprn_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderLocationsCommand(10012002, 123, _userId);

            var sut = new BulkDeleteProviderLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.Ukprn);
        }

        [Test]
        public async Task ValidateLarsCode_InValid_ReturnsError()
        {
            var command = new BulkDeleteProviderLocationsCommand(10012002, 123, _userId);
            var sut = new BulkDeleteProviderLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.LarsCode);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public async Task ValidateUserId_Empty_ReturnsError(string userId)
        {
            var command = new BulkDeleteProviderLocationsCommand(10012002, 123, userId);
            var sut = new BulkDeleteProviderLocationsCommandValidator(Mock.Of<IProviderReadRepository>(), Mock.Of<IProviderCourseReadRepository>());

            var result = await sut.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(c => c.UserId);
        }
    }
}
